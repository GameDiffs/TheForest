using System;
using System.Collections.Generic;
using System.Reflection;
using UniLinq;
using UnityEngine;

namespace AmplifyColor
{
	[Serializable]
	public class VolumeEffectComponent
	{
		public string componentName;

		public List<VolumeEffectField> fields;

		public VolumeEffectComponent(string name)
		{
			this.componentName = name;
			this.fields = new List<VolumeEffectField>();
		}

		public VolumeEffectComponent(Component c, VolumeEffectComponentFlags compFlags) : this(compFlags.componentName)
		{
			foreach (VolumeEffectFieldFlags current in compFlags.componentFields)
			{
				if (current.blendFlag)
				{
					FieldInfo field = c.GetType().GetField(current.fieldName);
					VolumeEffectField volumeEffectField = (!VolumeEffectField.IsValidType(field.FieldType.FullName)) ? null : new VolumeEffectField(field, c);
					if (volumeEffectField != null)
					{
						this.fields.Add(volumeEffectField);
					}
				}
			}
		}

		public VolumeEffectField AddField(FieldInfo pi, Component c)
		{
			return this.AddField(pi, c, -1);
		}

		public VolumeEffectField AddField(FieldInfo pi, Component c, int position)
		{
			VolumeEffectField volumeEffectField = (!VolumeEffectField.IsValidType(pi.FieldType.FullName)) ? null : new VolumeEffectField(pi, c);
			if (volumeEffectField != null)
			{
				if (position < 0 || position >= this.fields.Count)
				{
					this.fields.Add(volumeEffectField);
				}
				else
				{
					this.fields.Insert(position, volumeEffectField);
				}
			}
			return volumeEffectField;
		}

		public void RemoveEffectField(VolumeEffectField field)
		{
			this.fields.Remove(field);
		}

		public void UpdateComponent(Component c, VolumeEffectComponentFlags compFlags)
		{
			foreach (VolumeEffectFieldFlags fieldFlags in compFlags.componentFields)
			{
				if (fieldFlags.blendFlag)
				{
					if (!this.fields.Exists((VolumeEffectField s) => s.fieldName == fieldFlags.fieldName))
					{
						FieldInfo field = c.GetType().GetField(fieldFlags.fieldName);
						VolumeEffectField volumeEffectField = (!VolumeEffectField.IsValidType(field.FieldType.FullName)) ? null : new VolumeEffectField(field, c);
						if (volumeEffectField != null)
						{
							this.fields.Add(volumeEffectField);
						}
					}
				}
			}
		}

		public VolumeEffectField GetEffectField(string fieldName)
		{
			return this.fields.Find((VolumeEffectField s) => s.fieldName == fieldName);
		}

		public static FieldInfo[] ListAcceptableFields(Component c)
		{
			if (c == null)
			{
				return new FieldInfo[0];
			}
			FieldInfo[] source = c.GetType().GetFields();
			return (from f in source
			where VolumeEffectField.IsValidType(f.FieldType.FullName)
			select f).ToArray<FieldInfo>();
		}

		public string[] GetFieldNames()
		{
			return (from r in this.fields
			select r.fieldName).ToArray<string>();
		}
	}
}
