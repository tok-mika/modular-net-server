namespace com.tok.mika.projects.mws.pluginLoader
{
    using com.tok.mika.libs.mws.module;
    using System.Reflection;
    using System.Runtime.Loader;

    internal class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver? _resolver;
        internal libs.mws.module.Module? module;
        internal int? level;

        internal PluginLoadContext(string pluginPath)
            : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
            level = null;
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if(_resolver == null) return null;
            string? path = _resolver.ResolveAssemblyToPath(assemblyName);
            if (path != null)
            {
                return LoadFromAssemblyPath(path);
            }
            path = null;
            _resolver = null;
            return null;
        }

        internal void unload()
        {
            module = null;
            level = null;
            _resolver = null;
        }
    }
}
