using Newtonsoft.Json;

namespace CSharpPluginExample.Host
{
	public class DeserializeMeSomethingHost
	{
		public T DeserializeMeThis<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}
	}
}