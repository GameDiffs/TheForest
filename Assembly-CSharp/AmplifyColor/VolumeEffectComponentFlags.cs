using System;
using System.Collections.Generic;
using System.Reflection;
using UniLinq;
using UnityEngine;

namespace AmplifyColor
{
	[Serializable]
	public class VolumeEffectComponentFlags
	{
		public string componentName;

		public List<VolumeEffectFieldFlags> componentFields;

		public bool blendFlag;

		public VolumeEffectComponentFlags(string name)
		{
			this.componentName = name;
			this.componentFields = new List<VolumeEffectFieldFlags>();
		}

		public VolumeEffectComponentFlags(VolumeEffectComponent comp) : this(comp.componentName)
		{
			this.blendFlag = true;
			foreach (VolumeEffectField current in comp.fields)
			{
				if (VolumeEffectField.IsValidType(current.fieldType))
				{
					this.componentFields.Add(new VolumeEffectFieldFlags(current));
				}
			}
		}

		public VolumeEffectComponentFlags(Component c) : this(c.GetType() + string.Empty)
		{
			FieldInfo[] fields = c.GetType().GetFields();
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = array[i];
				if (VolumeEffectField.IsValidType(fieldInfo.FieldType.FullName))
				{
					this.componentFields.Add(new VolumeEffectFieldFlags(fieldInfo));
				}
			}
		}

		public void UpdateComponentFlags(VolumeEffectComponent comp)
		{
			foreach (VolumeEffectField field in comp.fields)
			{
				if (this.componentFields.Find((VolumeEffectFieldFlags s) => s.fieldName == field.fieldName) == null && VolumeEffectField.IsValidType(field.fieldType))
				{
					this.componentFields.Add(new VolumeEffectFieldFlags(field));
				}
			}
		}

		public void UpdateComponentFlags(Component c)
		{
			FieldInfo[] fields = c.GetType().GetFields();
			FieldInfo[] array = fields;
			FieldInfo pi;
			for (int i = 0; i < array.Length; i++)
			{
				pi = array[i];
				if (!this.componentFields.Exists((VolumeEffectFieldFlags s) => s.fieldName == pi.Name) && VolumeEffectField.IsValidType(pi.FieldType.FullName))
				{
					this.componentFields.Add(new VolumeEffectFieldFlags(pi));
				}
			}
		}

		public string[] GetFieldNames()
		{
			return (from r in this.componentFields
			where r.blendFlag
			select r.fieldName).ToArray<string>();
		}
	}
}
