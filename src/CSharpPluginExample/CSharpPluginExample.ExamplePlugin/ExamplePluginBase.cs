using System;
using System.Threading.Tasks;
using CSharpPluginExample.PluginBase;

namespace CSharpPluginExample.ExamplePlugin
{
	public class ExamplePluginBase : Plugin
	{
		public override string Name { get; } = "Fancy Example Plugin Name";

		public override async Task Start()
		{
			Console.WriteLine("Example plugin Start()");
			await Task.Delay(1000);
		}

		public override async Task Stop()
		{
			Console.WriteLine("Example plugin Stop()");
			await Task.Delay(1000);
		}
	}
}
