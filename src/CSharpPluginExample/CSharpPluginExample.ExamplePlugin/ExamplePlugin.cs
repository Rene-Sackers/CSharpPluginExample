using System;
using CSharpPluginExample.Host;

namespace CSharpPluginExample.ExamplePlugin
{
	public class ExamplePlugin : Plugin
	{
		public override void Start()
		{
			EventClass.ExampleEvent += ExampleEvent;
		}

		public override void Stop()
		{
			EventClass.ExampleEvent -= ExampleEvent;
		}

		private void ExampleEvent()
		{
			Console.WriteLine("Example event inside plugin");
		}
	}
}
