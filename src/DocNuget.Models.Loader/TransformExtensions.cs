using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Framework.Logging;

using Mono.Cecil;
using NuGet;

using ILogger = Microsoft.Framework.Logging.ILogger;

namespace DocNuget.Models.Loader {
    internal static class TransformExtensions {
        public static Package ToPackage(this ZipPackage zipPackage, List<string> otherVersions, ILogger logger) {
            return new Package {
                Id = zipPackage.Id,
                Title = zipPackage.Title ?? zipPackage.Id,
                Version = zipPackage.Version.ToString(),
                Versions = otherVersions,
                Summary = zipPackage.Summary ?? zipPackage.Description,
                DependencySets = zipPackage.DependencySets
                    .Select(set => set?.ToDependencySet())
                    .ToList(),
                Assemblies = zipPackage.GetFiles()
                    .Where(file => file.Path.StartsWith("lib", StringComparison.OrdinalIgnoreCase))
                    .Where(file => file.Path.EndsWith("dll", StringComparison.OrdinalIgnoreCase))
                    .GroupBy(file => Path.GetFileNameWithoutExtension(file.Path.Split('\\').Last()))
                    .Select(assembly => assembly.ToAssembly().TryLoadHelp(zipPackage, logger))
                    .ToList()
            };
        }

        public static Assembly TryLoadHelp(this Assembly assembly, ZipPackage zipPackage, ILogger logger) {
            logger.LogInformation("Looking for xml help for {0}", assembly.Name);
            var helpFile = zipPackage.GetFiles().FirstOrDefault(file => file.Path.Equals(assembly.Path.Replace("dll", "xml"), StringComparison.OrdinalIgnoreCase)); 

            if (helpFile != null) {
                logger.LogInformation("Found xml help for {0}", assembly.Name);
                var xml = XDocument.Load(helpFile.GetStream());
                foreach (var member in xml.Element("doc").Element("members").Elements("member")) {
                    var name = member.Attribute("name")?.Value;
                    if (name == null) {
                        continue;
                    }

                    logger.LogDebug("Found xml help for member {0}", name);
                    try {
                        switch (name.First()) {
                            case 'T': {
                                var type = assembly.FindType(name.Substring(2));
                                if (type != null) {
                                    type.Summary = member.Descendants("summary").FirstOrDefault()?.Value;
                                }
                                break;
                            }

                            case 'M': {
                                var argStart = name.IndexOf('(');
                                if (argStart < 2) {
                                    continue;
                                }
                                var methodName = name.Substring(2, argStart - 2);
                                var typeName = methodName.Substring(0, methodName.LastIndexOf('.'));
                                methodName = methodName.Substring(methodName.LastIndexOf('.') + 1);
                                var type = assembly.FindType(typeName);
                                if (type != null) {
                                    var method = type.Methods.FirstOrDefault(m => m.Name == methodName);
                                    if (method != null) {
                                        method.Summary = member.Descendants("summary").FirstOrDefault()?.Value;
                                    }
                                }
                                break;
                            }
                        }
                    } catch (Exception ex) {
                        logger.LogError("Failed to pull out help details", ex);
                    }
                }
            }

            return assembly;
        }

        public static IEnumerable<Type> GetAllTypes(this Assembly assembly) {
            return assembly.RootNamespace.GetAllTypes();
        }

        public static IEnumerable<Type> GetAllTypes(this Namespace @namespace) {
            return @namespace.Types.Concat(@namespace.Namespaces.SelectMany(GetAllTypes));
        }

        public static Type FindType(this Assembly assembly, string name) {
            return assembly.GetAllTypes().FirstOrDefault(type => type.FullName == name);
        }

        public static DependencySet ToDependencySet(this PackageDependencySet set) {
            return new DependencySet {
                TargetFramework = set.TargetFramework?.ToFramework(),
                SupportedFrameworks = set.SupportedFrameworks
                    .Select(framework => framework?.ToFramework())
                    .ToList(),
                Dependencies = set.Dependencies
                    .Select(dependency => dependency?.ToPackageDependency())
                    .ToList(),
            };
        }

        public static PackageDependency ToPackageDependency(this NuGet.PackageDependency dependency) {
            return new PackageDependency {
                Id = dependency.Id,
                VersionSpec = dependency.VersionSpec?.ToString(),
            };
        }

        public static Framework ToFramework(this System.Runtime.Versioning.FrameworkName framework) {
            return new Framework {
                FullName = framework.FullName,
                Identifier = framework.Identifier,
                Profile = framework.Profile,
                Version = framework.Version?.ToString(),
            };
        }

        public static Assembly ToAssembly(this IGrouping<string, IPackageFile> files) {
            var file = files.First();
            var reflectedAssembly = AssemblyDefinition.ReadAssembly(file.GetStream());

            var types = reflectedAssembly.Modules.SelectMany(module => module.Types).Where(type => type.IsPublic);

            var assembly = new Assembly {
                Name = files.Key,
                Path = file.Path,
                TargetFramework = file.TargetFramework?.ToFramework(),
                TargetFrameworks = files.Select(f => f.TargetFramework?.ToFramework())
                    .Where(f => f != null)
                    .ToList(),
                SupportedFrameworks = file.SupportedFrameworks
                    .Select(ToFramework)
                    .ToList(),
                RootNamespace = ToNamespaceTree(types, reflectedAssembly),
            };

            return assembly;
        }

        public static Namespace ToNamespaceTree(this IEnumerable<TypeDefinition> types, AssemblyDefinition assembly) {
            var namespaces = types.Select(type => type.Namespace).Where(ns => ns != "").Distinct();

            var root = new Namespace {
                Name = "<root>",
                FullName = "",
                Namespaces = new List<Namespace>(),
                Types = new List<Type>(),
            };

            foreach (var @namespace in namespaces) {
                Insert(root, @namespace.Split('.'));
            }

            foreach (var group in types.GroupBy(type => type.Namespace)) {
                var @namespace = Walk(root, group.Key.Split('.').Where(val => val != ""));
                @namespace.Types = group.Select(type => type.ToType(assembly)).ToList();
            }

            return root;
        }

        public static Type ToType(this TypeDefinition type, AssemblyDefinition assembly) {
            var baseType = type.BaseType?.ToTypeRef(assembly);
            var interfaces = type.Interfaces?.Select(@interface => @interface.ToTypeRef(assembly)).ToList();
            var allBaseTypes = baseType == null || baseType.FullName == "System.Object" || baseType.FullName == "System.ValueType"
                ? interfaces
                : new[] { baseType }.Concat(interfaces).ToList();

            return new Type {
                Name = CommonName(type.FullName) ?? type.Name,
                FullName = type.FullName,
                BaseType = baseType,
                Interfaces = interfaces,
                AllBaseTypes = allBaseTypes,
                InAssembly = type.Module.Assembly == assembly,
                GenericParameters = type.GenericParameters.Select(param => param.ToTypeRef(assembly)).ToList(),
                Methods = type.Methods
                    .Where(method => !(method.IsConstructor || method.IsSetter || method.IsGetter))
                    .Select(method => method.ToMethod(assembly))
                    .ToList(),
                Constructors = type.Methods
                    .Where(method => method.IsConstructor)
                    .Select(method => method.ToMethod(assembly))
                    .ToList(),
            };
        }

        public static TypeRef ToTypeRef(this TypeReference type, AssemblyDefinition assembly) {
            var typeRef = type.FullName.ToTypeRef() ?? new TypeRef {
                FullName = type.FullName,
                GenericParameters = type.GenericParameters.Select(param => param.ToTypeRef(assembly)).ToList(),
            };
            typeRef.Name = CommonName(type.FullName) ?? type.Name;
            typeRef.InAssembly = type.TryResolve()?.Module?.Assembly == assembly;
            return typeRef;
        }

        public static TypeRef ToTypeRef(this string name) {
            var match = Regex.Match(name, @"^(?<fullname>(?:(?<namespace>[^<]+)\.)?(?<name>[^<.]+))(?:<(?<generics>(?:(?<open><)|(?<close-open>>)|[^<>]+)+)>)?$");
            if (match.Success) {
                return new TypeRef {
                    Name = match.Groups["name"].Value,
                    FullName = match.Groups["fullname"].Value,
                    GenericParameters = Regex.Matches(
                        match.Groups["generics"].Value,
                        @"[^<>,]+(?:<(?:(?<open><)|(?<close-open>>)|[^<>]+)+>)?(?:, ?|$)")
                        .OfType<Match>()
                        .Select(m => m.Value.TrimEnd(',', ' ').ToTypeRef()).Where(r => r != null).ToList(),
                };
            } else {
                return null;
            }
        }

        public static TypeDefinition TryResolve(this TypeReference type) {
            try {
                return type.Resolve();
            } catch {
                return null;
            }
        }

        public static Method ToMethod(this MethodDefinition method, AssemblyDefinition assembly) {
            return new Method {
                Name = method.Name,
                FullName = method.FullName,
                ReturnType = method.ReturnType.ToTypeRef(assembly),
                Parameters = method.Parameters.Select(parameter => parameter.ToParameter(assembly)).ToList(),
                GenericParameters = method.GenericParameters.Select(parameter => parameter.ToGenericParameter(assembly)).ToList(),
                IsStatic = method.IsStatic,
                Visibility = method.IsPublic ? "public" : method.IsPrivate ? "private" : "<unknown>",
            };
        }

        public static Parameter ToParameter(this ParameterDefinition parameter, AssemblyDefinition assembly) {
            return new Parameter {
                Name = parameter.Name,
                Type = parameter.ParameterType.ToTypeRef(assembly),
                Default = parameter.Constant,
            };
        }

        public static GenericParameter ToGenericParameter(this Mono.Cecil.GenericParameter parameter, AssemblyDefinition assembly) {
            return new GenericParameter {
                Name = parameter.Name,
                Constraints = parameter.Constraints.Select(constraint => constraint.ToTypeRef(assembly)).ToList(),
            };
        }

        public static void Insert(Namespace @namespace, IEnumerable<string> path) {
            if (!path.Any()) {
                return;
            }

            var match = @namespace.Namespaces.FirstOrDefault(ns => ns.Name == path.First());
            if (match == null) {
                match = new Namespace {
                    Name = path.First(),
                    FullName = (@namespace.FullName == "" ? "" : @namespace.FullName + ".") + path.First(),
                    Namespaces = new List<Namespace>(),
                    Types = new List<Type>(),
                };
                @namespace.Namespaces.Add(match);
            }

            Insert(@match, path.Skip(1));
        }

        public static Namespace Walk(Namespace @namespace, IEnumerable<string> path) {
            if (!path.Any()) {
                return @namespace;
            }

            return Walk(@namespace.Namespaces.FirstOrDefault(ns => ns.Name == path.First()), path.Skip(1));
        }

        private static string CommonName(string name) {
            switch (name) {
                case "System.Void": return "void";
                case "System.Boolean": return "boolean";
                case "System.Object": return "object";
                default: return null;
            }
        }
    }
}
