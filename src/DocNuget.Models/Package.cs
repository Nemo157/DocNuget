using System.Collections.Generic;

namespace DocNuget.Models {
    public class Package {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Version { get; set; }

        public List<string> Versions { get; set; }

        public List<Assembly> Assemblies { get; set; }
    }

    public class Assembly {
        public string Name { get; set; }

        public string Framework { get; set; }

        public List<string> Frameworks { get; set; }

        public Package Package { get; set; }

        public Namespace RootNamespace { get; set; }
    }

    public class Namespace {
        public string Name { get; set; }

        public string FullName { get; set; }

        public Assembly Assembly { get; set; }

        public List<Namespace> Namespaces { get; set; }

        public List<Type> Types { get; set; }
    }

    public class Type {
        public string Name { get; set; }

        public Namespace Namespace { get; set; }
    }
}
