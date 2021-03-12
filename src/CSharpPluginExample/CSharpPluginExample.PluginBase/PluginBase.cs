using System.Threading.Tasks;

namespace CSharpPluginExample.PluginBase
{
	public abstract class Plugin
	{
		public abstract string Name { get; }

		public abstract Task Start();

		public abstract Task Stop();
	}
}
