using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CSharpPluginExample.Host.Helpers;

namespace CSharpPluginExample.Host
{
	/// <summary>
	/// Docs:
	/// https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support
	/// https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability
	/// </summary>
	public class Program
	{
		private static readonly Type PluginType = typeof(Plugin);
		private static readonly EventClass EventClass = new();

		public static async Task Main(string[] args)
		{
			Console.ForegroundColor = ConsoleColor.Gray;

			var pluginDllPath = Path.GetFullPath("./plugin/net5.0/CSharpPluginExample.ExamplePlugin.dll");
			if (!File.Exists(pluginDllPath))
			{
				Console.WriteLine("Plugin DLL missing at path: " + pluginDllPath);
				Console.ReadKey();
				return;
			}

			await ExecuteAndUnload(pluginDllPath);

			await Task.Delay(-1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)] // Doesn't seem to have effect, but it's recommended
		private static async Task ExecuteAndUnload(string pluginDllPath)
		{
			var pluginAssemblyName = Path.GetFileNameWithoutExtension(pluginDllPath);

			// Load the plugin
			var loadContext = new PluginLoadContext("Example plugin", pluginDllPath);
			loadContext.LoadRequiredFrameworkAssemblies(); // This makes JSON.Net work with unloading
			var loadContextWeakRef = new WeakReference(loadContext, true);
			var pluginAssembly = loadContext.LoadFromAssemblyName(new(pluginAssemblyName));

			// Get type references
			var pluginType = pluginAssembly.GetTypes().First(t => PluginType.IsAssignableFrom(t));
			var pluginInstance = Activator.CreateInstance(pluginType) as Plugin; // This is our ExamplePlugin.cs

			// Set shared class
			pluginInstance.EventClass = EventClass;

			// Interaction
			pluginInstance.Start();
			EventClass.Trigger();
			pluginInstance.Stop();

			// Unload stuff. If any of these lines are missing, the plugin will not unload
			//pluginInstance.EventClass = null; // This is the only line that can be removed, but I'll leave it for good measure
			pluginInstance = null;
			pluginAssembly = null;
			pluginType = null;

			loadContext.Unload();
			loadContext = null; // Make sure to set the load context to null after calling .Unload()

			Console.WriteLine("Unloading plugin");

			// If we don't do this, the plugin will not unload
			await Task.Delay(1);

			// Call GC until the weak reference says it's no longer alive, or we hit 10 cycles
			for (var i = 0; loadContextWeakRef.IsAlive && i < 10; i++)
			{
				Console.WriteLine($"Garbage collection iteration: {i}");
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}

			Console.ForegroundColor = loadContextWeakRef.IsAlive ? ConsoleColor.Red : ConsoleColor.Green;
			Console.WriteLine($"GC done. Is alive: {loadContextWeakRef.IsAlive} ({(loadContextWeakRef.IsAlive ? "Failed to unload" : "Unloaded successfully")})");
		}
	}
}
