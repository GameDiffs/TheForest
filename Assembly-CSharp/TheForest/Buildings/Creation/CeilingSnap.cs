using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	public class CeilingSnap : MonoBehaviour
	{
		public LayerMask _wallLayers;

		public float _raycastDistance = 5f;

		private Vector3 _offsetWithPlacer;

		[DebuggerHidden]
		private IEnumerator Start()
		{
			CeilingSnap.<Start>c__Iterator130 <Start>c__Iterator = new CeilingSnap.<Start>c__Iterator130();
			<Start>c__Iterator.<>f__this = this;
			return <Start>c__Iterator;
		}

		private void Update()
		{
			RaycastHit value;
			if (Physics.Raycast(LocalPlayer.MainCamTr.position, LocalPlayer.MainCamTr.forward, out value, this._raycastDistance, this._wallLayers.value))
			{
				bool flag = (double)value.normal.y < -0.75;
				base.transform.parent = null;
				base.transform.position = value.point;
				if (flag)
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
				LocalPlayer.Create.BuildingPlacer.SetNotclear();
				Scene.HudGui.PlaceIcon.SetActive(false);
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
