#if NETCOREAPP
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace TechTalk.SpecFlow.Specs.MSBuild.Analyzer
{
    public class AssemblyResolver : IDisposable
    {
        private readonly DependencyContext _dependencyContext;

        private readonly CompositeCompilationAssemblyResolver _resolver;

        private readonly AssemblyLoadContext _loadContext;

        public AssemblyResolver(string path)
        {
            // This loads the primary test assembly, but none of its dependencies.
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);

            _dependencyContext = DependencyContext.Load(assembly);

            _resolver = new CompositeCompilationAssemblyResolver(
                new ICompilationAssemblyResolver[]
                {
                    new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(path)),
                    new ReferenceAssemblyPathResolver(),
                    new PackageCompilationAssemblyResolver()
                });

            _loadContext = AssemblyLoadContext.GetLoadContext(assembly);

            _loadContext.Resolving += ResolvingAssembly;
        }

        public void Dispose()
        {
            _loadContext.Resolving -= ResolvingAssembly;
        }

        private Assembly ResolvingAssembly(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            bool NamesMatch(RuntimeLibrary runtime) => string.Equals(runtime.Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase);

            // Fast lookup: find a library with a name like the assembly-name (most match) and then attempt to load the library.
            var library = _dependencyContext.RuntimeLibraries.FirstOrDefault(NamesMatch);
            if (library != null && TryLoadAssembly(library, assemblyName, out var assembly))
            {
                return assembly;
            }

            // Slow lookup: look at every single package to try and find the matching assembly name.
            foreach (var runtime in _dependencyContext.RuntimeLibraries)
            {
                if (runtime.GetDefaultAssemblyNames(_dependencyContext).Any(name => name.Name == assemblyName.Name))
                {
                    if (TryLoadAssembly(runtime, assemblyName, out var assembly2))
                    {
                        return assembly2;
                    }
                }
            }

            return null;
        }

        private bool TryLoadAssembly(RuntimeLibrary library, AssemblyName assemblyName, out Assembly assembly)
        {
            var wrapper = new CompilationLibrary(
                library.Type,
                library.Name,
                library.Version,
                library.Hash,
                library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                library.Dependencies,
                library.Serviceable);

            var assemblyPaths = new List<string>();

            if (_resolver.TryResolveAssemblyPaths(wrapper, assemblyPaths))
            {
                foreach (var path in assemblyPaths)
                {
                    assembly = _loadContext.LoadFromAssemblyPath(path);

                    if (AssemblyName.ReferenceMatchesDefinition(assemblyName, assembly.GetName()))
                    {
                        return true;
                    }
                }
            }

            assembly = null;
            return false;
        }
    }
}
#endif
