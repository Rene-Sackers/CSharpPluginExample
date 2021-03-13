namespace CSharpPluginExample.Host
{
	public class EventClass
	{
		public delegate void ExampleEventHandler();

		public ExampleEventHandler ExampleEvent;

		internal void Trigger() => ExampleEvent?.Invoke();
	}
}
