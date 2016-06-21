using Pathfinding.Serialization.JsonFx;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.Serialization
{
	public class LayerMaskConverter : JsonConverter
	{
		public override bool CanConvert(Type type)
		{
			return object.Equals(type, typeof(LayerMask));
		}

		public override object ReadJson(Type type, Dictionary<string, object> values)
		{
			return (int)values["value"];
		}

		public override Dictionary<string, object> WriteJson(Type type, object value)
		{
			return new Dictionary<string, object>
			{
				{
					"value",
					((LayerMask)value).value
				}
			};
		}
	}
}
