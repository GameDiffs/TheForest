using System;
using System.Collections.Generic;
using System.Reflection;
using UniLinq;
using UnityEngine;

namespace AmplifyColor
{
	[Serializable]
	public class VolumeEffect
	{
		public AmplifyColorBase gameObject;

		public List<VolumeEffectComponent> components;

		public VolumeEffect(AmplifyColorBase effect)
		{
			this.gameObject = effect;
			this.components = new List<VolumeEffectComponent>();
		}

		public static VolumeEffect BlendValuesToVolumeEffect(VolumeEffectFlags flags, VolumeEffect volume1, VolumeEffect volume2, float blend)
		{
			VolumeEffect volumeEffect = new VolumeEffect(volume1.gameObject);
			foreach (VolumeEffectComponentFlags compFlags in flags.components)
			{
				if (compFlags.blendFlag)
				{
					VolumeEffectComponent volumeEffectComponent = volume1.components.Find((VolumeEffectComponent s) => s.componentName == compFlags.componentName);
					VolumeEffectComponent volumeEffectComponent2 = volume2.components.Find((VolumeEffectComponent s) => s.componentName == compFlags.componentName);
					if (volumeEffectComponent != null && volumeEffectComponent2 != null)
					{
						VolumeEffectComponent volumeEffectComponent3 = new VolumeEffectComponent(volumeEffectComponent.componentName);
						foreach (VolumeEffectFieldFlags fieldFlags in compFlags.componentFields)
						{
							if (fieldFlags.blendFlag)
							{
								VolumeEffectField volumeEffectField = volumeEffectComponent.fields.Find((VolumeEffectField s) => s.fieldName == fieldFlags.fieldName);
								VolumeEffectField volumeEffectField2 = volumeEffectComponent2.fields.Find((VolumeEffectField s) => s.fieldName == fieldFlags.fieldName);
								if (volumeEffectField != null && volumeEffectField2 != null)
								{
									VolumeEffectField volumeEffectField3 = new VolumeEffectField(volumeEffectField.fieldName, volumeEffectField.fieldType);
									string fieldType = volumeEffectField3.fieldType;
									switch (fieldType)
									{
									case "System.Single":
										volumeEffectField3.valueSingle = Mathf.Lerp(volumeEffectField.valueSingle, volumeEffectField2.valueSingle, blend);
										break;
									case "System.Boolean":
										volumeEffectField3.valueBoolean = volumeEffectField2.valueBoolean;
										break;
									case "UnityEngine.Vector2":
										volumeEffectField3.valueVector2 = Vector2.Lerp(volumeEffectField.valueVector2, volumeEffectField2.valueVector2, blend);
										break;
									case "UnityEngine.Vector3":
										volumeEffectField3.valueVector3 = Vector3.Lerp(volumeEffectField.valueVector3, volumeEffectField2.valueVector3, blend);
										break;
									case "UnityEngine.Vector4":
										volumeEffectField3.valueVector4 = Vector4.Lerp(volumeEffectField.valueVector4, volumeEffectField2.valueVector4, blend);
										break;
									case "UnityEngine.Color":
										volumeEffectField3.valueColor = Color.Lerp(volumeEffectField.valueColor, volumeEffectField2.valueColor, blend);
										break;
									}
									volumeEffectComponent3.fields.Add(volumeEffectField3);
								}
							}
						}
						volumeEffect.components.Add(volumeEffectComponent3);
					}
				}
			}
			return volumeEffect;
		}

		public VolumeEffectComponent AddComponent(Component c, VolumeEffectComponentFlags compFlags)
		{
			if (compFlags == null)
			{
				VolumeEffectComponent volumeEffectComponent = new VolumeEffectComponent(c.GetType() + string.Empty);
				this.components.Add(volumeEffectComponent);
				return volumeEffectComponent;
			}
			VolumeEffectComponent volumeEffectComponent2;
			if ((volumeEffectComponent2 = this.components.Find((VolumeEffectComponent s) => s.componentName == c.GetType() + string.Empty)) != null)
			{
				volumeEffectComponent2.UpdateComponent(c, compFlags);
				return volumeEffectComponent2;
			}
			VolumeEffectComponent volumeEffectComponent3 = new VolumeEffectComponent(c, compFlags);
			this.components.Add(volumeEffectComponent3);
			return volumeEffectComponent3;
		}

		public void RemoveEffectComponent(VolumeEffectComponent comp)
		{
			this.components.Remove(comp);
		}

		public void UpdateVolume()
		{
			if (this.gameObject == null)
			{
				return;
			}
			VolumeEffectFlags effectFlags = this.gameObject.EffectFlags;
			foreach (VolumeEffectComponentFlags current in effectFlags.components)
			{
				if (current.blendFlag)
				{
					Component component = this.gameObject.GetComponent(current.componentName);
					if (component != null)
					{
						this.AddComponent(component, current);
					}
				}
			}
		}

		public void SetValues(AmplifyColorBase targetColor)
		{
			VolumeEffectFlags effectFlags = targetColor.EffectFlags;
			GameObject gameObject = targetColor.gameObject;
			foreach (VolumeEffectComponentFlags compFlags in effectFlags.components)
			{
				if (compFlags.blendFlag)
				{
					Component component = gameObject.GetComponent(compFlags.componentName);
					VolumeEffectComponent volumeEffectComponent = this.components.Find((VolumeEffectComponent s) => s.componentName == compFlags.componentName);
					if (!(component == null) && volumeEffectComponent != null)
					{
						foreach (VolumeEffectFieldFlags fieldFlags in compFlags.componentFields)
						{
							if (fieldFlags.blendFlag)
							{
								FieldInfo field = component.GetType().GetField(fieldFlags.fieldName);
								VolumeEffectField volumeEffectField = volumeEffectComponent.fields.Find((VolumeEffectField s) => s.fieldName == fieldFlags.fieldName);
								if (field != null && volumeEffectField != null)
								{
									string fullName = field.FieldType.FullName;
									switch (fullName)
									{
									case "System.Single":
										field.SetValue(component, volumeEffectField.valueSingle);
										break;
									case "System.Boolean":
										field.SetValue(component, volumeEffectField.valueBoolean);
										break;
									case "UnityEngine.Vector2":
										field.SetValue(component, volumeEffectField.valueVector2);
										break;
									case "UnityEngine.Vector3":
										field.SetValue(component, volumeEffectField.valueVector3);
										break;
									case "UnityEngine.Vector4":
										field.SetValue(component, volumeEffectField.valueVector4);
										break;
									case "UnityEngine.Color":
										field.SetValue(component, volumeEffectField.valueColor);
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		public void BlendValues(AmplifyColorBase targetColor, VolumeEffect other, float blendAmount)
		{
			VolumeEffectFlags effectFlags = targetColor.EffectFlags;
			GameObject gameObject = targetColor.gameObject;
			foreach (VolumeEffectComponentFlags compFlags in effectFlags.components)
			{
				if (compFlags.blendFlag)
				{
					Component component = gameObject.GetComponent(compFlags.componentName);
					VolumeEffectComponent volumeEffectComponent = this.components.Find((VolumeEffectComponent s) => s.componentName == compFlags.componentName);
					VolumeEffectComponent volumeEffectComponent2 = other.components.Find((VolumeEffectComponent s) => s.componentName == compFlags.componentName);
					if (!(component == null) && volumeEffectComponent != null && volumeEffectComponent2 != null)
					{
						foreach (VolumeEffectFieldFlags fieldFlags in compFlags.componentFields)
						{
							if (fieldFlags.blendFlag)
							{
								FieldInfo field = component.GetType().GetField(fieldFlags.fieldName);
								VolumeEffectField volumeEffectField = volumeEffectComponent.fields.Find((VolumeEffectField s) => s.fieldName == fieldFlags.fieldName);
								VolumeEffectField volumeEffectField2 = volumeEffectComponent2.fields.Find((VolumeEffectField s) => s.fieldName == fieldFlags.fieldName);
								if (field != null && volumeEffectField != null && volumeEffectField2 != null)
								{
									string fullName = field.FieldType.FullName;
									switch (fullName)
									{
									case "System.Single":
										field.SetValue(component, Mathf.Lerp(volumeEffectField.valueSingle, volumeEffectField2.valueSingle, blendAmount));
										break;
									case "System.Boolean":
										field.SetValue(component, volumeEffectField2.valueBoolean);
										break;
									case "UnityEngine.Vector2":
										field.SetValue(component, Vector2.Lerp(volumeEffectField.valueVector2, volumeEffectField2.valueVector2, blendAmount));
										break;
									case "UnityEngine.Vector3":
										field.SetValue(component, Vector3.Lerp(volumeEffectField.valueVector3, volumeEffectField2.valueVector3, blendAmount));
										break;
									case "UnityEngine.Vector4":
										field.SetValue(component, Vector4.Lerp(volumeEffectField.valueVector4, volumeEffectField2.valueVector4, blendAmount));
										break;
									case "UnityEngine.Color":
										field.SetValue(component, Color.Lerp(volumeEffectField.valueColor, volumeEffectField2.valueColor, blendAmount));
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		public VolumeEffectComponent GetEffectComponent(string compName)
		{
			return this.components.Find((VolumeEffectComponent s) => s.componentName == compName);
		}

		public static Component[] ListAcceptableComponents(AmplifyColorBase go)
		{
			if (go == null)
			{
				return new Component[0];
			}
			Component[] source = go.GetComponents(typeof(Component));
			return (from comp in source
			where comp != null && (!(comp.GetType() + string.Empty).StartsWith("UnityEngine.") && comp.GetType() != typeof(AmplifyColorBase))
			select comp).ToArray<Component>();
		}

		public string[] GetComponentNames()
		{
			return (from r in this.components
			select r.componentName).ToArray<string>();
		}
	}
}
