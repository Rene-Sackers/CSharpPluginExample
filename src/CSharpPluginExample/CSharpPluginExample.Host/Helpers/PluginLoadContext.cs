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
			Console.WriteLine($"Load assembly {name.FullName}");

			var assemblyPath = _resolver.ResolveAssemblyToPath(name);
			if (assemblyPath != null)
				Console.WriteLine($"\tPath: {assemblyPath}");
			else
				Console.WriteLine("\tCouldn't resolve to path");

			return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
		}

		protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
		{
			Console.WriteLine($"Load unmanaged {unmanagedDllName}");

			var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
			if (libraryPath != null)
				Console.WriteLine($"\tPath: {libraryPath}");
			else
				Console.WriteLine("\tCouldn't resolve to path");

			return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
		}
	}
}