using Pathfinding.Serialization.JsonFx;
using Pathfinding.Util;
using System;
using System.Collections.Generic;

namespace Pathfinding.Serialization
{
	public class GuidConverter : JsonConverter
	{
		public override bool CanConvert(Type type)
		{
			return object.Equals(type, typeof(Pathfinding.Util.Guid));
		}

		public override object ReadJson(Type objectType, Dictionary<string, object> values)
		{
			string str = (string)values["value"];
			return new Pathfinding.Util.Guid(str);
		}

		public override Dictionary<string, object> WriteJson(Type type, object value)
		{
			Pathfinding.Util.Guid guid = (Pathfinding.Util.Guid)value;
			return new Dictionary<string, object>
			{
				{
					"value",
					guid.ToString()
				}
			};
		}
	}
}
