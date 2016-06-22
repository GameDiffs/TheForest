using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	public class VerticalSnapSwap : MonoBehaviour
	{
		public enum RayCastOrigins
		{
			Player,
			Building
		}

		public LayerMask _layers;

		public VerticalSnapSwap.RayCastOrigins _origin;

		public float _raycastDistance = 5f;

		public GameObject _snappedGo;

		public GameObject _freeGo;

		public bool _initializeSnapped;

		public bool _initializeAirborne;

		private Vector3 _offsetWithPlacer;

		private Renderer _snappedRenderer;

		private Renderer _freeRenderer;

		[DebuggerHidden]
		private IEnumerator Start()
		{
			VerticalSnapSwap.<Start>c__Iterator145 <Start>c__Iterator = new VerticalSnapSwap.<Start>c__Iterator145();
			<Start>c__Iterator.<>f__this = this;
			return <Start>c__Iterator;
		}

		private void Update()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast((this._origin != VerticalSnapSwap.RayCastOrigins.Player) ? base.transform.position : LocalPlayer.MainCamTr.position, LocalPlayer.MainCamTr.forward, out raycastHit, this._raycastDistance, this._layers.value))
			{
				bool flag = (double)Mathf.Abs(raycastHit.normal.y) < 0.5;
				if (flag)
				{
					if (this._origin == VerticalSnapSwap.RayCastOrigins.Player)
					{
						base.transform.parent = null;
						base.transform.position = raycastHit.point;
					}
					if (flag)
					{
						Vector3 normal = raycastHit.normal;
						normal.y = 0f;
						base.transform.rotation = Quaternion.Euler(0f, LocalPlayer.Create.BuildingPlacer.transform.rotation.y, 0f) * Quaternion.LookRotation(normal);
					}
					this._snappedGo.SetActive(true);
					this._freeGo.SetActive(false);
					LocalPlayer.Create.BuildingPlacer.SetRenderer(this._snappedRenderer);
				}
				else if (this._snappedGo.activeSelf)
				{
					this._snappedGo.SetActive(false);
					this._freeGo.SetActive(true);
					LocalPlayer.Create.BuildingPlacer.SetRenderer(this._freeRenderer);
					base.transform.parent = LocalPlayer.Create.BuildingPlacer.transform;
					base.transform.localPosition = this._offsetWithPlacer;
				}
			}
			else if (!this._freeGo.activeSelf)
			{
				this._snappedGo.SetActive(false);
				this._freeGo.SetActive(true);
				LocalPlayer.Create.BuildingPlacer.SetRenderer(this._freeRenderer);
				base.transform.parent = LocalPlayer.Create.BuildingPlacer.transform;
				base.transform.localPosition = this._offsetWithPlacer;
			}
		}

		[DebuggerHidden]
		private IEnumerator OnPlaced()
		{
			VerticalSnapSwap.<OnPlaced>c__Iterator146 <OnPlaced>c__Iterator = new VerticalSnapSwap.<OnPlaced>c__Iterator146();
			<OnPlaced>c__Iterator.<>f__this = this;
			return <OnPlaced>c__Iterator;
		}
	}
}
