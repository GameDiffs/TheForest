using System;
using UnityEngine;

namespace TheForest.Utils
{
	public class NameFromEnumIndex : PropertyAttribute
	{
		public Type EnumType
		{
			get;
			private set;
		}

		public NameFromEnumIndex(Type enumType)
		{
			this.EnumType = enumType;
		}
	}
}
