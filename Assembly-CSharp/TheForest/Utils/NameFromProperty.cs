using System;
using UnityEngine;

namespace TheForest.Utils
{
	public class NameFromProperty : PropertyAttribute
	{
		public string NameProperty
		{
			get;
			private set;
		}

		public NameFromProperty(string nameProperty)
		{
			this.NameProperty = nameProperty;
		}
	}
}
