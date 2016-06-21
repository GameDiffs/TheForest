using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.UI
{
	public class CarriedWeightDisplay : MonoBehaviour
	{
		public Material _selectedMaterial;

		private bool _hovered;

		private Material _normalMaterial;

		private void Start()
		{
			if (!this._hovered)
			{
				base.enabled = false;
			}
		}

		private void OnDisable()
		{
			if (this._hovered)
			{
				Scene.HudGui.HideCarriedWeightInfo();
				base.gameObject.GetComponent<Renderer>().sharedMaterial = this._normalMaterial;
				this._hovered = false;
			}
			base.enabled = false;
		}

		private void OnMouseExitCollider()
		{
			if (base.enabled)
			{
				base.enabled = false;
			}
		}

		private void OnMouseEnterCollider()
		{
			if (!base.enabled)
			{
				this._hovered = true;
				base.enabled = true;
				if (base.gameObject.GetComponent<Renderer>() && this._normalMaterial != base.gameObject.GetComponent<Renderer>().sharedMaterial)
				{
					this._normalMaterial = base.gameObject.GetComponent<Renderer>().sharedMaterial;
				}
				base.gameObject.GetComponent<Renderer>().sharedMaterial = this._selectedMaterial;
				Scene.HudGui.ShowCarriedWeightInfo(LocalPlayer.InventoryCam.WorldToViewportPoint(base.transform.position));
			}
		}
	}
}
