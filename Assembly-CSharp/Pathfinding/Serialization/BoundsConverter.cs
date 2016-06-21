using Pathfinding.Serialization.JsonFx;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.Serialization
{
	public class BoundsConverter : JsonConverter
	{
		public override bool CanConvert(Type type)
		{
			return object.Equals(type, typeof(Bounds));
		}

		public override object ReadJson(Type objectType, Dictionary<string, object> values)
		{
			return new Bounds
			{
				center = new Vector3(base.CastFloat(values["cx"]), base.CastFloat(values["cy"]), base.CastFloat(values["cz"])),
				extents = new Vector3(base.CastFloat(values["ex"]), base.CastFloat(values["ey"]), base.CastFloat(values["ez"]))
			};
		}

		public override Dictionary<string, object> WriteJson(Type type, object value)
		{
			Bounds bounds = (Bounds)value;
			return new Dictionary<string, object>
			{
				{
					"cx",
					bounds.center.x
				},
				{
					"cy",
					bounds.center.y
				},
				{
					"cz",
					bounds.center.z
				},
				{
					"ex",
					bounds.extents.x
				},
				{
					"ey",
					bounds.extents.y
				},
				{
					"ez",
					bounds.extents.z
				}
			};
		}
	}
}
