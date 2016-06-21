using Pathfinding.Serialization.JsonFx;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.Serialization
{
	public class VectorConverter : JsonConverter
	{
		public override bool CanConvert(Type type)
		{
			return object.Equals(type, typeof(Vector2)) || object.Equals(type, typeof(Vector3)) || object.Equals(type, typeof(Vector4));
		}

		public override object ReadJson(Type type, Dictionary<string, object> values)
		{
			if (object.Equals(type, typeof(Vector2)))
			{
				return new Vector2(base.CastFloat(values["x"]), base.CastFloat(values["y"]));
			}
			if (object.Equals(type, typeof(Vector3)))
			{
				return new Vector3(base.CastFloat(values["x"]), base.CastFloat(values["y"]), base.CastFloat(values["z"]));
			}
			if (object.Equals(type, typeof(Vector4)))
			{
				return new Vector4(base.CastFloat(values["x"]), base.CastFloat(values["y"]), base.CastFloat(values["z"]), base.CastFloat(values["w"]));
			}
			throw new NotImplementedException("Can only read Vector2,3,4. Not objects of type " + type);
		}

		public override Dictionary<string, object> WriteJson(Type type, object value)
		{
			if (object.Equals(type, typeof(Vector2)))
			{
				Vector2 vector = (Vector2)value;
				return new Dictionary<string, object>
				{
					{
						"x",
						vector.x
					},
					{
						"y",
						vector.y
					}
				};
			}
			if (object.Equals(type, typeof(Vector3)))
			{
				Vector3 vector2 = (Vector3)value;
				return new Dictionary<string, object>
				{
					{
						"x",
						vector2.x
					},
					{
						"y",
						vector2.y
					},
					{
						"z",
						vector2.z
					}
				};
			}
			if (object.Equals(type, typeof(Vector4)))
			{
				Vector4 vector3 = (Vector4)value;
				return new Dictionary<string, object>
				{
					{
						"x",
						vector3.x
					},
					{
						"y",
						vector3.y
					},
					{
						"z",
						vector3.z
					},
					{
						"w",
						vector3.w
					}
				};
			}
			throw new NotImplementedException("Can only write Vector2,3,4. Not objects of type " + type);
		}
	}
}
