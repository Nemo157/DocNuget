using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mono.Cecil;
using NuGet;

namespace DocNuget.Models.Loader {
    internal static class TransformExtensions {
        public static Package ToPackage(this ZipPackage zipPackage, List<string> otherVersions) {
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
                    .Where(file => file.Path.StartsWith("lib") && file.Path.EndsWith("dll"))
                    .GroupBy(file => Path.GetFileNameWithoutExtension(file.Path.Split('\\').Last()))
                    .Select(assembly => assembly?.ToAssembly())
                    .ToList()
            };
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
            return new TypeRef {
                Name = CommonName(type.FullName) ?? type.Name,
                FullName = type.FullName,
                InAssembly = type.Resolve()?.Module?.Assembly == assembly,
            };
        }

        public static Method ToMethod(this MethodDefinition method, AssemblyDefinition assembly) {
            return new Method {
                Name = method.Name,
                FullName = method.FullName,
                ReturnType = method.ReturnType.ToTypeRef(assembly),
                Parameters = method.Parameters.Select(parameter => parameter.ToParameter(assembly)).ToList(),
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

        public static Package Link(this Package package) {
            foreach (var assembly in package.Assemblies) {
                assembly.Package = package;
                Link(assembly, assembly.RootNamespace);
            }

            return package;
        }

        private static void Link(Assembly assembly, Namespace @namespace) {
            @namespace.Assembly = assembly;
            foreach (var childNamespace in @namespace.Namespaces) {
                Link(assembly, childNamespace);
            }

            foreach (var type in @namespace.Types) {
                type.Namespace = @namespace;
                type.Assembly = assembly;

                Link(assembly, type.BaseType);
                Link(assembly, type.Interfaces);
                Link(assembly, type.GenericArguments);
                Link(assembly, type.Methods.SelectMany(method => new[] { method.ReturnType }.Concat(method.Parameters.Select(parameter => parameter.Type))));
            }
        }

        private static void Link(Assembly assembly, TypeRef type) {
            if (type == null) {
                return;
            }
            if (type.InAssembly) {
                type.Assembly = assembly;
            }
            Link(assembly, type.GenericArguments);
        }

        private static void Link(Assembly assembly, IEnumerable<TypeRef> types) {
            if (types == null) {
                return;
            }
            foreach (var type in types) {
                Link(assembly, type);
            }
        }

        private static string CommonName(string name) {
            switch (name) {
                case "System.Void": return "void";
                case "System.Boolean": return "boolean";
                default: return null;
            }
        }
    }
}
