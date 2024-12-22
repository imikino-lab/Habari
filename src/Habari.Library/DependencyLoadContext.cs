using System.Reflection;
using System.Runtime.Loader;

namespace Habari;

public class DependencyLoadContext : AssemblyLoadContext
{
    private AssemblyDependencyResolver assemblyDependencyResolver;

    public DependencyLoadContext(string dependencyFilename)
    {
        assemblyDependencyResolver = new AssemblyDependencyResolver(dependencyFilename);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        string? path = assemblyDependencyResolver.ResolveAssemblyToPath(assemblyName);
        return !string.IsNullOrWhiteSpace(path) ? LoadFromAssemblyPath(path) : null;
    }
}
