﻿namespace CSharpPluginExample.Host
{
	public abstract class Plugin
	{
		public EventClass EventClass { get; internal set; }

		public DeserializeMeSomething DeserializeMeSomething { get; internal set; }

		public abstract void Start();

		public abstract void Stop();
	}
}
