using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	public class VerticalSnap : MonoBehaviour
	{
		public enum RayCastOrigins
		{
			Player,
			Building
		}

		public bool _verticalOnly;

		public LayerMask _wallLayers;

		public VerticalSnap.RayCastOrigins _origin;

		public float _raycastDistance = 5f;

		private Vector3 _offsetWithPlacer;

		[DebuggerHidden]
		private IEnumerator Start()
		{
			VerticalSnap.<Start>c__Iterator13D <Start>c__Iterator13D = new VerticalSnap.<Start>c__Iterator13D();
			<Start>c__Iterator13D.<>f__this = this;
			return <Start>c__Iterator13D;
		}

		private void Update()
		{
			RaycastHit value;
			if (Physics.Raycast((this._origin != VerticalSnap.RayCastOrigins.Player) ? base.transform.position : LocalPlayer.MainCamTr.position, LocalPlayer.MainCamTr.forward, out value, this._raycastDistance, this._wallLayers.value))
			{
				bool flag = (double)Mathf.Abs(value.normal.y) < 0.5;
				if (this._origin == VerticalSnap.RayCastOrigins.Player)
				{
					base.transform.parent = null;
					base.transform.position = value.point;
				}
				if (!this._verticalOnly)
				{
					if (this._origin == VerticalSnap.RayCastOrigins.Player)
					{
						base.transform.rotation = Quaternion.Euler(0f, LocalPlayer.Create.BuildingPlacer.transform.rotation.y, 0f) * Quaternion.FromToRotation(Vector3.up, value.normal);
					}
				}
				else if (flag)
				{
					Vector3 normal = value.normal;
					normal.y = 0f;
					base.transform.rotation = Quaternion.Euler(0f, LocalPlayer.Create.BuildingPlacer.transform.rotation.y, 0f) * Quaternion.LookRotation(normal);
				}
				if (!this._verticalOnly || flag)
				{
					LocalPlayer.Create.BuildingPlacer.LastHit = new RaycastHit?(value);
					LocalPlayer.Create.BuildingPlacer.SetClear();
					Scene.HudGui.PlaceIcon.SetActive(true);
				}
				else
				{
					LocalPlayer.Create.BuildingPlacer.SetNotclear();
					Scene.HudGui.PlaceIcon.SetActive(false);
				}
			}
			else if (base.transform.parent == null)
			{
				base.transform.parent = LocalPlayer.Create.BuildingPlacer.transform;
				base.transform.localPosition = this._offsetWithPlacer;
				if (this._verticalOnly)
				{
					LocalPlayer.Create.BuildingPlacer.SetNotclear();
					Scene.HudGui.PlaceIcon.SetActive(false);
				}
			}
			else
			{
				LocalPlayer.Create.BuildingPlacer.SetNotclear();
				Scene.HudGui.PlaceIcon.SetActive(false);
			}
		}

		private void OnPlaced()
		{
			base.enabled = false;
		}

		private void OnDeserialized()
		{
			base.enabled = false;
		}
	}
}
