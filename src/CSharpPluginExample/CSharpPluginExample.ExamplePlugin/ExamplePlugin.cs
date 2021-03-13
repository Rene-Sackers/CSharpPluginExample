using System;
using CSharpPluginExample.Host;
using Newtonsoft.Json;

namespace CSharpPluginExample.ExamplePlugin
{
	public class JsonDeserializeType
	{
		public string Property { get; set; }
	}

	public class ExamplePlugin : Plugin
	{
		private const string SampleJson = @"{ ""Property"": ""asdf"" }";

		public override void Start()
		{
			EventClass.ExampleEvent += ExampleEvent;
			DeserializeWithJsonNet();
			//DeserializeWithMsJson();
		}

		private void DeserializeWithJsonNet()
		{
			// This only works because of NewtonsoftIsolationHelper.cs
			var deserialized = JsonConvert.DeserializeObject<JsonDeserializeType>(SampleJson);
			Console.WriteLine($"Deserialized with JSON.Net: {deserialized.Property}");
		}

		private void DeserializeWithMsJson()
		{
			// If we do this, the plugin will not unload
			var deserialized = System.Text.Json.JsonSerializer.Deserialize<JsonDeserializeType>(SampleJson);
			Console.WriteLine($"Deserialized with MS.Net: {deserialized.Property}");
		}

		public override void Stop()
		{
			// If we do not unhook event handlers, the plugin will not unload
			EventClass.ExampleEvent -= ExampleEvent;
		}

		private void ExampleEvent()
		{
			Console.WriteLine("Example event inside plugin");
		}
	}
}
