using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CSharpPluginExample.Host.Helpers;

namespace CSharpPluginExample.Host
{
	public class Program
	{
		private static readonly Type PluginType = typeof(Plugin);

		public static async Task Main(string[] args)
		{
			var pluginDllPath = Path.GetFullPath("./plugin/net5.0/CSharpPluginExample.ExamplePlugin.dll");
			if (!File.Exists(pluginDllPath))
			{
				EndMessage("Plugin DLL missing at path: " + pluginDllPath);
				return;
			}

			await ExecuteAndUnload(pluginDllPath);

			await Task.Delay(-1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static async Task ExecuteAndUnload(string pluginDllPath)
		{
			var eventClass = new EventClass();

			var pluginAssemblyName = Path.GetFileNameWithoutExtension(pluginDllPath);
			var loadContext = new PluginLoadContext("Example plugin", pluginDllPath);
			var loadContextWeakRef = new WeakReference(loadContext, true);

			// Literally load, then unload the assembly. NOTHING in between
			var pluginAssembly = loadContext.LoadFromAssemblyName(new(pluginAssemblyName));
			var pluginType = pluginAssembly.GetTypes().First(t => PluginType.IsAssignableFrom(t));
			var pluginInstance = Activator.CreateInstance(pluginType) as Plugin;

			pluginInstance.EventClass = eventClass;
			pluginInstance.Start();
			eventClass.Trigger();
			pluginInstance.Stop();
			pluginInstance.EventClass = null;
			pluginInstance = null;
			pluginAssembly = null;
			pluginType = null;

			loadContext.Unload();
			loadContext = null;

			Console.WriteLine("Unloading plugin");

			// If I don't do this, pluginAssemblyWeakRef.IsAlive stays true
			await Task.Delay(1);

			for (var i = 0; loadContextWeakRef.IsAlive && i < 10; i++)
			{
				Console.WriteLine($"Garbage collection iteration: {i}");
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}

			Console.WriteLine($"GC done. Is alive: {loadContextWeakRef.IsAlive}");
		}

		private static void EndMessage(string message)
		{
			Console.WriteLine(message);
			Console.ReadKey();
		}
	}
}
