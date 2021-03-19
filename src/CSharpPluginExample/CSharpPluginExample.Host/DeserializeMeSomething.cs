using Newtonsoft.Json;

namespace CSharpPluginExample.Host
{
	public class DeserializeMeSomething
	{
		public T DeserializeMeThis<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}
	}
}