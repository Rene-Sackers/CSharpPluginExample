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
			//DeserializeWithProvidedType();
			//DeserializeWithSelfMadeType();
			DeserializeWithLocalType();
		}

		private void DeserializeWithProvidedType()
		{
			// If we do this, the plugin will not unload
			var result = DeserializeMeSomethingHost.DeserializeMeThis<JsonDeserializeType>(SampleJson);
			Console.WriteLine($"Deserialized with provided type: {result.Property}");
		}

		private void DeserializeWithSelfMadeType()
		{
			// If we do this, the plugin will not unload
			// If we move the type DeserializeMeSomething into the plugin project, it'll work just fine (see next method)
			var result = new DeserializeMeSomethingHost().DeserializeMeThis<JsonDeserializeType>(SampleJson);
			Console.WriteLine($"Deserialized with self made type: {result.Property}");

			/*
				HandleTable:
				    0000026B055C1360 (strong handle)
				    -> 0000026B1D8B5420 System.Object[]
				    -> 0000026B058BD3D8 Newtonsoft.Json.Serialization.DefaultContractResolver
				    -> 0000026B058BD588 Newtonsoft.Json.Utilities.ThreadSafeStore`2[[System.Type, System.Private.CoreLib],[Newtonsoft.Json.Serialization.JsonContract, Newtonsoft.Json]]
				    -> 0000026B058BD5A8 System.Collections.Concurrent.ConcurrentDictionary`2[[System.Type, System.Private.CoreLib],[Newtonsoft.Json.Serialization.JsonContract, Newtonsoft.Json]]
				    -> 0000026B058BD958 System.Collections.Concurrent.ConcurrentDictionary`2+Tables[[System.Type, System.Private.CoreLib],[Newtonsoft.Json.Serialization.JsonContract, Newtonsoft.Json]]
				    -> 0000026B058BD848 System.Collections.Concurrent.ConcurrentDictionary`2+Node[[System.Type, System.Private.CoreLib],[Newtonsoft.Json.Serialization.JsonContract, Newtonsoft.Json]][]
				    -> 0000026B058EAF20 System.Collections.Concurrent.ConcurrentDictionary`2+Node[[System.Type, System.Private.CoreLib],[Newtonsoft.Json.Serialization.JsonContract, Newtonsoft.Json]]
				    -> 0000026B058CE320 System.RuntimeType
				    -> 0000026B058BAD50 System.Reflection.LoaderAllocator

				    0000026B055C13E8 (strong handle)
				    -> 0000026B1D8B1018 System.Object[]
				    -> 0000026B058C1448 Newtonsoft.Json.Utilities.ThreadSafeStore`2[[System.Object, System.Private.CoreLib],[Newtonsoft.Json.JsonContainerAttribute, Newtonsoft.Json]]
				    -> 0000026B058C1468 System.Collections.Concurrent.ConcurrentDictionary`2[[System.Object, System.Private.CoreLib],[Newtonsoft.Json.JsonContainerAttribute, Newtonsoft.Json]]
				    -> 0000026B058C1800 System.Collections.Concurrent.ConcurrentDictionary`2+Tables[[System.Object, System.Private.CoreLib],[Newtonsoft.Json.JsonContainerAttribute, Newtonsoft.Json]]
				    -> 0000026B058C16F0 System.Collections.Concurrent.ConcurrentDictionary`2+Node[[System.Object, System.Private.CoreLib],[Newtonsoft.Json.JsonContainerAttribute, Newtonsoft.Json]][]
				    -> 0000026B058E9EB0 System.Collections.Concurrent.ConcurrentDictionary`2+Node[[System.Object, System.Private.CoreLib],[Newtonsoft.Json.JsonContainerAttribute, Newtonsoft.Json]]
				    -> 0000026B058CE320 System.RuntimeType
				    -> 0000026B058BAD50 System.Reflection.LoaderAllocator
			 */
		}

		private void DeserializeWithLocalType()
		{
			// This works (contrary to method above)
			var result = new DeserializeMeSomethingLocal().DeserializeMeThis<JsonDeserializeType>(SampleJson);
			Console.WriteLine($"Deserialized with local type: {result.Property}");
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

			/*
				HandleTable:
				    00000189A0CD1320 (strong handle)
				    -> 00000189B8EC9C10 System.Object[]
				    -> 00000189A0EF5C08 System.Text.Json.JsonSerializerOptions
				    -> 00000189A0EF5C88 System.Collections.Concurrent.ConcurrentDictionary`2[[System.Type, System.Private.CoreLib],[System.Text.Json.Serialization.JsonConverter, System.Text.Json]]
				    -> 00000189A0EF6020 System.Collections.Concurrent.ConcurrentDictionary`2+Tables[[System.Type, System.Private.CoreLib],[System.Text.Json.Serialization.JsonConverter, System.Text.Json]]
				    -> 00000189A0EF5F10 System.Collections.Concurrent.ConcurrentDictionary`2+Node[[System.Type, System.Private.CoreLib],[System.Text.Json.Serialization.JsonConverter, System.Text.Json]][]
				    -> 00000189A0EF7068 System.Collections.Concurrent.ConcurrentDictionary`2+Node[[System.Type, System.Private.CoreLib],[System.Text.Json.Serialization.JsonConverter, System.Text.Json]]
				    -> 00000189A0EDEA48 System.RuntimeType
				    -> 00000189A0ECAD28 System.Reflection.LoaderAllocator
			 */
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
