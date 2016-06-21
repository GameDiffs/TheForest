using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic, RequireComponent(typeof(GUITexture))]
	public class LastBuiltLocation : MonoBehaviour
	{
		public Transform target;

		public Vector3 offset = Vector3.up;

		public bool Showing;

		public bool IsOverLayIcon = true;

		private Vector3 relativePosition;

		private bool isAboveTerrain;

		private void Start()
		{
			Vector3 worldPosition = (!this.target) ? ((!base.transform.parent) ? base.transform.position : base.transform.parent.position) : this.target.position;
			float num = Terrain.activeTerrain.SampleHeight(worldPosition) + Terrain.activeTerrain.transform.position.y - 3f;
			this.isAboveTerrain = (worldPosition.y > num);
			this.IsOverLayIcon = !Scene.IsInSinkhole((!base.transform.parent) ? base.transform.position : base.transform.parent.position);
		}

		private void LateUpdate()
		{
			if (LocalPlayer.Inventory && Scene.Atmosphere)
			{
				if (this.IsOverLayIcon && (!PlayerPreferences.ShowOverlayIcons || Clock.InCave == this.isAboveTerrain))
				{
					this.Showing = false;
				}
				else if (LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.World != this.Showing)
				{
					this.Showing = !this.Showing;
				}
				if (this.target == null)
				{
					if (base.transform.parent != null)
					{
						this.target = base.transform.parent;
					}
					else
					{
						this.target = base.transform;
					}
					this.Showing = false;
					base.gameObject.GetComponent<GUITexture>().enabled = false;
				}
				if (this.Showing && LocalPlayer.MainCam)
				{
					this.relativePosition = LocalPlayer.MainCamTr.InverseTransformPoint(this.target.position);
					this.relativePosition.z = Mathf.Max(this.relativePosition.z, 0.1f);
					base.transform.position = LocalPlayer.MainCam.WorldToViewportPoint(LocalPlayer.MainCamTr.TransformPoint(this.relativePosition + this.offset));
					if (!base.gameObject.GetComponent<GUITexture>().enabled)
					{
						base.gameObject.GetComponent<GUITexture>().enabled = true;
					}
				}
				else if (base.gameObject.GetComponent<GUITexture>().enabled)
				{
					base.gameObject.GetComponent<GUITexture>().enabled = false;
				}
			}
		}
	}
}
