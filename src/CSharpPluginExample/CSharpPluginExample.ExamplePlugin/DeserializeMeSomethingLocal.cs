using Newtonsoft.Json;

namespace CSharpPluginExample.ExamplePlugin
{
	public class DeserializeMeSomethingLocal
	{
		public T DeserializeMeThis<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}
	}
}