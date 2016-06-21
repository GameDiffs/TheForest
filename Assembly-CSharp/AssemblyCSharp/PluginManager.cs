using System;
using System.Collections.Generic;
using System.IO;

namespace AssemblyCSharp
{
	public class PluginManager
	{
		private const string _dir = "Plugins";

		private Stack<Plugin> _plugins = new Stack<Plugin>();

		public Plugin[] Plugins
		{
			get
			{
				return this._plugins.ToArray();
			}
		}

		public PluginManager()
		{
			if (Directory.Exists("Plugins"))
			{
				string[] files = Directory.GetFiles("Plugins", "*.dll");
				string[] array = files;
				for (int i = 0; i < array.Length; i++)
				{
					string fileName = array[i];
					this._plugins.Push(new Plugin(fileName));
				}
			}
		}
	}
}
