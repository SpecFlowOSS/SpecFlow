#if NETCOREAPP

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    internal sealed class PluginAssemblyResolver
    {
        private readonly ICompilationAssemblyResolver _assemblyResolver;
        private readonly DependencyContext _dependencyContext;
        private readonly AssemblyLoadContext _loadContext;

        public Assembly Assembly { get; }

        public PluginAssemblyResolver(string path)
        {
            _loadContext = AssemblyLoadContext.GetLoadContext(typeof(PluginAssemblyResolver).Assembly);
            Assembly = _loadContext.LoadFromAssemblyPath(path);
            _dependencyContext = DependencyContext.Load(Assembly);

            _assemblyResolver = new CompositeCompilationAssemblyResolver(new ICompilationAssemblyResolver[]
            {
                new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(path)),
                new ReferenceAssemblyPathResolver(),
                new PackageCompilationAssemblyResolver()
            });

            _loadContext.Resolving += OnResolving;
            _loadContext.Unloading += OnUnloading;
        }

        private void OnUnloading(AssemblyLoadContext context)
        {
            _loadContext.Resolving -= OnResolving;
            _loadContext.Unloading -= OnUnloading;
        }

        private Assembly OnResolving(AssemblyLoadContext context, AssemblyName name)
        {
            var library = _dependencyContext.RuntimeLibraries.FirstOrDefault(
                runtimeLibrary => string.Equals(runtimeLibrary.Name, name.Name, StringComparison.OrdinalIgnoreCase));

            if (library != null)
            {
                var wrapper = new CompilationLibrary(
                    library.Type,
                    library.Name,
                    library.Version,
                    library.Hash,
                    library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                    library.Dependencies,
                    library.Serviceable);

                var assemblies = new List<string>();
                _assemblyResolver.TryResolveAssemblyPaths(wrapper, assemblies);
                
                if (assemblies.Count > 0)
                {
                    return _loadContext.LoadFromAssemblyPath(assemblies[0]);
                }
            }

            return null;
        }

        public static Assembly Load(string absolutePath)
        {
            return new PluginAssemblyResolver(absolutePath).Assembly;
        }
    }
}
#endif
