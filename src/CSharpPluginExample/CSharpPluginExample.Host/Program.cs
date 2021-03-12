using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CSharpPluginExample.Host.Helpers;
using CSharpPluginExample.PluginBase;

namespace CSharpPluginExample.Host
{
	public class Program
	{
		private static readonly Type PluginBaseType = typeof(Plugin);

		public static async Task Main(string[] args)
		{
			var pluginDllPath = Path.GetFullPath("./plugin/net5.0/CSharpPluginExample.ExamplePlugin.dll");
			if (!File.Exists(pluginDllPath))
			{
				EndMessage("Plugin DLL missing at path: " + pluginDllPath);
				return;
			}

			var pluginAssemblyName = Path.GetFileNameWithoutExtension(pluginDllPath);
			var loadContext = new PluginLoadContext("Example plugin", pluginDllPath);
			var pluginAssembly = loadContext.LoadFromAssemblyName(new(pluginAssemblyName));
			var pluginAssemblyWeakRef = new WeakReference(pluginAssembly);

			//var pluginBaseType = pluginAssembly.GetTypes().FirstOrDefault(t => PluginBaseType.IsAssignableFrom(t));
			//if (pluginBaseType == null)
			//{
			//	EndMessage("Plugin does not have a class implementing type " + PluginBaseType.FullName);
			//	loadContext.Unload();
			//	return;
			//}

			//var pluginBaseInstance = Activator.CreateInstance(pluginBaseType) as Plugin;
			//Console.WriteLine($"Plugin \"{pluginBaseInstance.Name}\" loaded.");
			//await pluginBaseInstance.Start();
			//await pluginBaseInstance.Stop();

			loadContext.Unload();
			Console.WriteLine("Unloading plugin");
			
			for (var i = 0; pluginAssemblyWeakRef.IsAlive && i < 10; i++)
			{
				Console.WriteLine($"Garbage collection iteration: {i}");
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}

			Console.WriteLine($"GC done. Is alive: {pluginAssemblyWeakRef.IsAlive}");

			await Task.Delay(-1);
		}

		private static void EndMessage(string message)
		{
			Console.WriteLine(message);
			Console.ReadKey();
		}
	}
}
