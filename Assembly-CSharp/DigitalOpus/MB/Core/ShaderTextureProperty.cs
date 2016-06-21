using System;
using System.Collections.Generic;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public class ShaderTextureProperty
	{
		public string name;

		public bool isNormalMap;

		public ShaderTextureProperty(string n, bool norm)
		{
			this.name = n;
			this.isNormalMap = norm;
		}

		public static string[] GetNames(List<ShaderTextureProperty> props)
		{
			string[] array = new string[props.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = props[i].name;
			}
			return array;
		}
	}
}
