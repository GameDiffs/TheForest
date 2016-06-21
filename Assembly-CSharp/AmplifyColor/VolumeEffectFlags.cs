using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace AmplifyColor
{
	[Serializable]
	public class VolumeEffectFlags
	{
		public List<VolumeEffectComponentFlags> components;

		public VolumeEffectFlags()
		{
			this.components = new List<VolumeEffectComponentFlags>();
		}

		public void AddComponent(Component c)
		{
			VolumeEffectComponentFlags volumeEffectComponentFlags;
			if ((volumeEffectComponentFlags = this.components.Find((VolumeEffectComponentFlags s) => s.componentName == c.GetType() + string.Empty)) != null)
			{
				volumeEffectComponentFlags.UpdateComponentFlags(c);
			}
			else
			{
				this.components.Add(new VolumeEffectComponentFlags(c));
			}
		}

		public void UpdateFlags(VolumeEffect effectVol)
		{
			foreach (VolumeEffectComponent comp in effectVol.components)
			{
				VolumeEffectComponentFlags volumeEffectComponentFlags;
				if ((volumeEffectComponentFlags = this.components.Find((VolumeEffectComponentFlags s) => s.componentName == comp.componentName)) == null)
				{
					this.components.Add(new VolumeEffectComponentFlags(comp));
				}
				else
				{
					volumeEffectComponentFlags.UpdateComponentFlags(comp);
				}
			}
		}

		public static void UpdateCamFlags(AmplifyColorBase[] effects, AmplifyColorVolumeBase[] volumes)
		{
			for (int i = 0; i < effects.Length; i++)
			{
				AmplifyColorBase amplifyColorBase = effects[i];
				amplifyColorBase.EffectFlags = new VolumeEffectFlags();
				for (int j = 0; j < volumes.Length; j++)
				{
					AmplifyColorVolumeBase amplifyColorVolumeBase = volumes[j];
					VolumeEffect volumeEffect = amplifyColorVolumeBase.EffectContainer.GetVolumeEffect(amplifyColorBase);
					if (volumeEffect != null)
					{
						amplifyColorBase.EffectFlags.UpdateFlags(volumeEffect);
					}
				}
			}
		}

		public VolumeEffect GenerateEffectData(AmplifyColorBase go)
		{
			VolumeEffect volumeEffect = new VolumeEffect(go);
			foreach (VolumeEffectComponentFlags current in this.components)
			{
				if (current.blendFlag)
				{
					Component component = go.GetComponent(current.componentName);
					if (component != null)
					{
						volumeEffect.AddComponent(component, current);
					}
				}
			}
			return volumeEffect;
		}

		public VolumeEffectComponentFlags GetComponentFlags(string compName)
		{
			return this.components.Find((VolumeEffectComponentFlags s) => s.componentName == compName);
		}

		public string[] GetComponentNames()
		{
			return (from r in this.components
			where r.blendFlag
			select r.componentName).ToArray<string>();
		}
	}
}
