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
        public string Name { get; set; }

        public string Path { get; set; }

        public Framework TargetFramework { get; set; }

        public List<Framework> TargetFrameworks { get; set; }

        public List<Framework> SupportedFrameworks { get; set; }

        public Namespace RootNamespace { get; set; }
    }

    public class Namespace {
        public string Name { get; set; }

        public string FullName { get; set; }

        public List<Namespace> Namespaces { get; set; }

        public List<Type> Types { get; set; }
    }

    public class Type {
        public string Name { get; set; }

        public string FullName { get; set; }

        public string Summary { get; set; }

        public TypeRef BaseType { get; set; }

        public List<TypeRef> AllBaseTypes { get; set; }

        public List<TypeRef> Interfaces { get; set; }

        public List<TypeRef> GenericParameters { get; set; }

        public bool InAssembly { get; set; }

        public List<Method> Methods { get; set; }

        public List<Method> Constructors { get; set; }
    }

    public class TypeRef {
        public string Name { get; set; }

        public string FullName { get; set; }

        public List<TypeRef> GenericParameters { get; set; }

        public bool InAssembly { get; set; }
    }

    public class Method {
        public string Name { get; set; }

        public string FullName { get; set; }

        public string Summary { get; set; }

        public TypeRef ReturnType { get; set; }

        public List<Parameter> Parameters { get; set; }

        public List<GenericParameter> GenericParameters { get; set; }

        public bool IsStatic { get; set; }

        public string Visibility { get; set; }
    }

    public class Parameter {
        public string Name { get; set; }

        public TypeRef Type { get; set; }

        public object Default { get; set; }
    }

    public class GenericParameter {
        public string Name { get; set; }

        public List<TypeRef> Constraints { get; set; }
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
