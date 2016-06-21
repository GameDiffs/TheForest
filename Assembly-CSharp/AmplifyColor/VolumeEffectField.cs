using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor
{
	[Serializable]
	public class VolumeEffectField
	{
		public string fieldName;

		public string fieldType;

		public float valueSingle;

		public Color valueColor;

		public bool valueBoolean;

		public Vector2 valueVector2;

		public Vector3 valueVector3;

		public Vector4 valueVector4;

		public VolumeEffectField(string fieldName, string fieldType)
		{
			this.fieldName = fieldName;
			this.fieldType = fieldType;
		}

		public VolumeEffectField(FieldInfo pi, Component c) : this(pi.Name, pi.FieldType.FullName)
		{
			object value = pi.GetValue(c);
			this.UpdateValue(value);
		}

		public static bool IsValidType(string type)
		{
			if (type != null)
			{
				if (VolumeEffectField.<>f__switch$map0 == null)
				{
					VolumeEffectField.<>f__switch$map0 = new Dictionary<string, int>(6)
					{
						{
							"System.Single",
							0
						},
						{
							"System.Boolean",
							0
						},
						{
							"UnityEngine.Color",
							0
						},
						{
							"UnityEngine.Vector2",
							0
						},
						{
							"UnityEngine.Vector3",
							0
						},
						{
							"UnityEngine.Vector4",
							0
						}
					};
				}
				int num;
				if (VolumeEffectField.<>f__switch$map0.TryGetValue(type, out num))
				{
					if (num == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void UpdateValue(object val)
		{
			string text = this.fieldType;
			switch (text)
			{
			case "System.Single":
				this.valueSingle = (float)val;
				break;
			case "System.Boolean":
				this.valueBoolean = (bool)val;
				break;
			case "UnityEngine.Color":
				this.valueColor = (Color)val;
				break;
			case "UnityEngine.Vector2":
				this.valueVector2 = (Vector2)val;
				break;
			case "UnityEngine.Vector3":
				this.valueVector3 = (Vector3)val;
				break;
			case "UnityEngine.Vector4":
				this.valueVector4 = (Vector4)val;
				break;
			}
		}
	}
}
