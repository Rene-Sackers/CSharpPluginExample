using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace CSharpPluginExample.Host.Helpers
{
	internal class PluginLoadContext : AssemblyLoadContext
	{
		private static readonly List<Assembly> LoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
		private static readonly Dictionary<PluginLoadContext, List<Assembly>> LoadedByPlugins = new();

		private readonly AssemblyDependencyResolver _resolver;

		public PluginLoadContext(string name, string pluginPath) : base(name, true)
		{
			_resolver = new(pluginPath);
			Unloading += OnUnloading;
		}

		private static void OnUnloading(AssemblyLoadContext context)
		{
			Console.WriteLine($"Unloading context {context.Name}");
		}

		protected override Assembly Load(AssemblyName name)
		{
			Console.WriteLine($"Loading assembly {name.FullName}");

			var assemblyPath = _resolver.ResolveAssemblyToPath(name);
			if (assemblyPath == null)
			{
				Console.WriteLine("\tUnable to resolve assembly to path");
				return null;
			}

			Console.WriteLine($"\tLoad assembly from path {assemblyPath}");
			return LoadFromAssemblyPath(assemblyPath);
		}

		protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
		{
			var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
			return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
		}
	}
}