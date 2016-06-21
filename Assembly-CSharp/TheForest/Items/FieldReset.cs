using System;
using UnityEngine;

namespace TheForest.Items
{
	public class FieldReset : PropertyAttribute
	{
		public object Value
		{
			get;
			set;
		}

		public FieldReset(object value)
		{
			this.Value = value;
		}
	}
}
