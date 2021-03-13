using System;
using System.Reflection;
using System.Runtime.Loader;

namespace CSharpPluginExample.Host.Helpers
{
	internal class PluginLoadContext : AssemblyLoadContext
	{
		private readonly AssemblyDependencyResolver _resolver;

		public PluginLoadContext(string name, string pluginPath) : base(name, true)
		{
			_resolver = new(pluginPath);
			Unloading += OnUnloading;
		}

		private static void OnUnloading(AssemblyLoadContext context)
		{
			Console.WriteLine($"Unloading context \"{context.Name}\"");
		}

		protected override Assembly Load(AssemblyName name)
		{
			var assemblyPath = _resolver.ResolveAssemblyToPath(name);
			return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
		}

		protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
		{
			var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
			return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
		}
	}
}