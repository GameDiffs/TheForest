using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[AddComponentMenu("Buildings/Creation/Create GUI")]
	public class CreateGui : MonoBehaviour
	{
		public Create.BuildingTypes _buildingType;

		public bool _blockInMP;

		public Material _blockedMaterial;

		private Material MyMat;

		private Color SwitchColor;

		private bool WasDisabled;

		public bool rename;

		private void Awake()
		{
			if (BoltNetwork.isRunning && this._blockInMP && this._blockedMaterial)
			{
				base.gameObject.GetComponent<Renderer>().sharedMaterial = this._blockedMaterial;
			}
			this.MyMat = base.gameObject.GetComponent<Renderer>().material;
			this.SwitchColor = this.MyMat.color;
			this.NotActive();
		}

		private void OnDisable()
		{
			this.WasDisabled = true;
		}

		private void OnDrawGizmosSelected()
		{
			if (this.rename)
			{
				this.rename = false;
				base.name = "GuiBuild" + this._buildingType;
			}
		}

		private void Update()
		{
			if (this.WasDisabled)
			{
				this.WasDisabled = false;
			}
			else if (!this._blockInMP || !BoltNetwork.isRunning)
			{
				if (this.ScreenRect().Contains(TheForest.Utils.Input.mousePosition))
				{
					this.Active();
					if (TheForest.Utils.Input.GetButtonDown("Fire1") || (TheForest.Utils.Input.IsGamePad && TheForest.Utils.Input.GetButtonDown("Take")))
					{
						LocalPlayer.Create.CreateBuilding(this._buildingType);
						this.NotActive();
						LocalPlayer.Create.CloseTheBook();
					}
				}
				else
				{
					this.NotActive();
				}
			}
			else
			{
				this.Active();
			}
		}

		private void Active()
		{
			this.SwitchColor.a = 0.2f;
			this.MyMat.color = this.SwitchColor;
		}

		private void NotActive()
		{
			this.SwitchColor.a = 0.05f;
			this.MyMat.color = this.SwitchColor;
		}

		public Rect ScreenRect()
		{
			BoxCollider component = base.GetComponent<BoxCollider>();
			Vector3 a = base.transform.TransformVector(component.size);
			Vector3 vector = LocalPlayer.MainCam.WorldToScreenPoint(base.transform.position - a / 2f);
			Vector3 vector2 = LocalPlayer.MainCam.WorldToScreenPoint(base.transform.position + a / 2f);
			return new Rect(vector2.x, vector2.y, Mathf.Abs(vector2.x - vector.x), Mathf.Abs(vector2.y - vector.y));
		}
	}
}
