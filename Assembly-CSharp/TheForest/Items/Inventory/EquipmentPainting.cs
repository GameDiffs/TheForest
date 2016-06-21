using System;
using UnityEngine;

namespace TheForest.Items.Inventory
{
	[DoNotSerializePublic, AddComponentMenu("Items/World/Equipement Painting (held)")]
	public class EquipmentPainting : MonoBehaviour
	{
		public enum Colors
		{
			Default,
			Green,
			Orange
		}

		public Material _greenMat;

		public Material _orangeMat;

		public Renderer[] _renderers;

		[SerializeThis]
		private EquipmentPainting.Colors _color;

		public EquipmentPainting.Colors Color
		{
			get
			{
				return this._color;
			}
			set
			{
				this._color = value;
			}
		}

		public void OnDeserialized()
		{
			EquipmentPainting.Colors color = this._color;
			if (color != EquipmentPainting.Colors.Green)
			{
				if (color == EquipmentPainting.Colors.Orange)
				{
					this.PaintInOrange();
				}
			}
			else
			{
				this.PaintInGreen();
			}
		}

		public void PaintInGreen()
		{
			this._color = EquipmentPainting.Colors.Green;
			Renderer[] renderers = this._renderers;
			for (int i = 0; i < renderers.Length; i++)
			{
				Renderer renderer = renderers[i];
				renderer.sharedMaterial = this._greenMat;
			}
			Bloodify component = base.GetComponent<Bloodify>();
			if (component)
			{
				UnityEngine.Object.Destroy(component);
			}
		}

		public void PaintInOrange()
		{
			this._color = EquipmentPainting.Colors.Orange;
			Renderer[] renderers = this._renderers;
			for (int i = 0; i < renderers.Length; i++)
			{
				Renderer renderer = renderers[i];
				renderer.sharedMaterial = this._orangeMat;
			}
			Bloodify component = base.GetComponent<Bloodify>();
			if (component)
			{
				UnityEngine.Object.Destroy(component);
			}
		}
	}
}
