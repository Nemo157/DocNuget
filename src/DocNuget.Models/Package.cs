using System.Collections.Generic;

namespace DocNuget.Models {
    public class Package {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Version { get; set; }

        public List<string> Versions { get; set; }

        public List<Assembly> Assemblies { get; set; }

        public List<DependencySet> DependencySets { get; set; }
    }

    public class Assembly {
        public Package Package { get; set; }

        public string Name { get; set; }

        public Framework TargetFramework { get; set; }

        public List<Framework> TargetFrameworks { get; set; }

        public List<Framework> SupportedFrameworks { get; set; }

        public Namespace RootNamespace { get; set; }
    }

    public class Namespace {
        public Assembly Assembly { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public List<Namespace> Namespaces { get; set; }

        public List<Type> Types { get; set; }
    }

    public class Type {
        public string Name { get; set; }

        public string FullName { get; set; }

        public string BaseType { get; set; }

        public List<string> Interfaces { get; set; }

        public Namespace Namespace { get; set; }
    }

    public class DependencySet {
        public Framework TargetFramework { get; set; }

        public List<Framework> SupportedFrameworks { get; set; }

        public List<PackageDependency> Dependencies { get; set; }
    }

    public class Framework {
        public string FullName { get; set; }

        public string Identifier { get; set; }

        public string Profile { get; set; }

        public string Version { get; set; }
    }

    public class PackageDependency {
        public string Id { get; set; }

        public string VersionSpec { get; set; }
    }
}
