using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            var reflectedAssembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(file.GetStream());

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
                RootNamespace = ToNamespaceTree(types),
            };

            return assembly;
        }

        public static Namespace ToNamespaceTree(this IEnumerable<Mono.Cecil.TypeDefinition> types) {
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
                @namespace.Types = group.Select(ToType).ToList();
            }

            return root;
        }

        public static Type ToType(this Mono.Cecil.TypeDefinition type) {
            return new Type {
                Name = type.Name,
                FullName = type.FullName,
                BaseType = type.BaseType?.FullName,
                Interfaces = type.Interfaces?.Select(@interface => @interface.FullName).ToList(),
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
            }
        }
    }
}
