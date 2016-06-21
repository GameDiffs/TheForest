using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	public class HorizontalSnapSwap : MonoBehaviour
	{
		public enum RayCastOrigins
		{
			Player,
			Placer
		}

		public LayerMask _layers;

		public HorizontalSnapSwap.RayCastOrigins _origin;

		public float _raycastDistance = 5f;

		public GameObject _snappedGo;

		public GameObject _freeGo;

		public bool _snappedIsAirborne;

		public bool _freeIsAirborne;

		public bool _snappedHasCustomPlace;

		public bool _freeHasCustomPlace;

		public bool _initializeSnapped;

		public bool _initializeAirborne;

		private Vector3 _offsetWithPlacer;

		private Renderer _snappedRenderer;

		private Renderer _freeRenderer;

		[DebuggerHidden]
		private IEnumerator Start()
		{
			HorizontalSnapSwap.<Start>c__Iterator137 <Start>c__Iterator = new HorizontalSnapSwap.<Start>c__Iterator137();
			<Start>c__Iterator.<>f__this = this;
			return <Start>c__Iterator;
		}

		private void Update()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(((this._origin != HorizontalSnapSwap.RayCastOrigins.Player) ? LocalPlayer.Create.BuildingPlacer.transform.position : LocalPlayer.MainCamTr.position) + Vector3.up, Vector3.down, out raycastHit, (!this._snappedGo.activeSelf) ? this._raycastDistance : (this._raycastDistance * 2f), this._layers.value))
			{
				bool flag = (double)Mathf.Abs(raycastHit.normal.y) > 0.5;
				if (flag)
				{
					if (this._origin == HorizontalSnapSwap.RayCastOrigins.Player)
					{
						base.transform.parent = null;
						base.transform.position = raycastHit.point;
					}
					if (flag)
					{
						raycastHit.normal.y = 0f;
						base.transform.localRotation = Quaternion.identity;
					}
					this._snappedGo.SetActive(true);
					this._freeGo.SetActive(false);
					if (this._snappedHasCustomPlace)
					{
						LocalPlayer.Create.Grabber.ClosePlace();
					}
					else
					{
						LocalPlayer.Create.Grabber.ShowPlace();
					}
					LocalPlayer.Create.BuildingPlacer.SetClear();
					LocalPlayer.Create.BuildingPlacer.SetRenderer(this._snappedRenderer);
					LocalPlayer.Create.BuildingPlacer.ApplyLeaningPosRot(base.transform, this._snappedIsAirborne, false);
					Scene.HudGui.LockPositionIcon.SetActive(false);
					Scene.HudGui.UnlockPositionIcon.SetActive(false);
					Scene.HudGui.ToggleAutoFillIcon.SetActive(false);
					Scene.HudGui.ToggleManualPlacementIcon.SetActive(false);
				}
				else if (this._snappedGo.activeSelf)
				{
					this._snappedGo.SetActive(false);
					this._freeGo.SetActive(true);
					if (this._freeHasCustomPlace)
					{
						LocalPlayer.Create.Grabber.ClosePlace();
					}
					else
					{
						LocalPlayer.Create.Grabber.ShowPlace();
					}
					LocalPlayer.Create.BuildingPlacer.Airborne = this._freeIsAirborne;
					LocalPlayer.Create.BuildingPlacer.SetRenderer(this._freeRenderer);
					base.transform.parent = LocalPlayer.Create.BuildingPlacer.transform;
					base.transform.localPosition = this._offsetWithPlacer;
					base.transform.localRotation = Quaternion.identity;
					Scene.HudGui.LockPositionIcon.SetActive(false);
					Scene.HudGui.UnlockPositionIcon.SetActive(false);
					Scene.HudGui.ToggleAutoFillIcon.SetActive(false);
					Scene.HudGui.ToggleManualPlacementIcon.SetActive(false);
				}
			}
			else if (!this._freeGo.activeSelf)
			{
				this._snappedGo.SetActive(false);
				this._freeGo.SetActive(true);
				if (this._freeHasCustomPlace)
				{
					LocalPlayer.Create.Grabber.ClosePlace();
				}
				else
				{
					LocalPlayer.Create.Grabber.ShowPlace();
				}
				LocalPlayer.Create.BuildingPlacer.Airborne = this._freeIsAirborne;
				LocalPlayer.Create.BuildingPlacer.SetRenderer(this._freeRenderer);
				base.transform.parent = LocalPlayer.Create.BuildingPlacer.transform;
				base.transform.localPosition = this._offsetWithPlacer;
				base.transform.localRotation = Quaternion.identity;
				Scene.HudGui.LockPositionIcon.SetActive(false);
				Scene.HudGui.UnlockPositionIcon.SetActive(false);
				Scene.HudGui.ToggleAutoFillIcon.SetActive(false);
				Scene.HudGui.ToggleManualPlacementIcon.SetActive(false);
			}
		}

		private void OnPlaced()
		{
			base.enabled = false;
			Transform child;
			if (this._snappedGo.activeSelf)
			{
				child = this._snappedGo.transform.GetChild(0);
			}
			else
			{
				child = this._freeGo.transform.GetChild(0);
			}
			if (!BoltNetwork.isRunning)
			{
				if (LocalPlayer.Create.BuildingPlacer.LastHit.HasValue && LocalPlayer.Create.BuildingPlacer.LastHit.Value.transform.GetComponentInParent<BoltEntity>())
				{
					child.parent = LocalPlayer.Create.BuildingPlacer.LastHit.Value.transform.GetComponentInParent<BoltEntity>().transform;
				}
				else
				{
					child.parent = null;
				}
				child.SendMessage("OnPlaced", SendMessageOptions.DontRequireReceiver);
				GameObject gameObject = child.FindChild("Trigger").gameObject;
				gameObject.SetActive(true);
				if ((this._snappedGo.activeSelf && this._initializeSnapped) || (this._freeGo.activeSelf && this._initializeAirborne))
				{
					gameObject.GetComponent<Craft_Structure>().Initialize();
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				CoopConstructionEx component = child.GetComponent<CoopConstructionEx>();
				if (component)
				{
					BoltEntity component2 = child.GetComponent<BoltEntity>();
					BoltEntity parentEntity = LocalPlayer.Create.GetParentEntity(child.gameObject);
					component.SendMessage("OnSerializing");
					CoopConstructionExToken coopConstructionExToken = LocalPlayer.Create.GetCoopConstructionExToken(component, parentEntity);
					PlaceFoundationEx placeFoundationEx = PlaceFoundationEx.Create(GlobalTargets.OnlyServer);
					placeFoundationEx.Parent = parentEntity;
					placeFoundationEx.Position = child.transform.position;
					placeFoundationEx.Prefab = component2.prefabId;
					placeFoundationEx.Token = coopConstructionExToken;
					placeFoundationEx.Send();
				}
				else
				{
					PlaceConstruction placeConstruction = PlaceConstruction.Create(GlobalTargets.OnlyServer);
					if (LocalPlayer.Create.BuildingPlacer.LastHit.HasValue)
					{
						placeConstruction.Parent = LocalPlayer.Create.BuildingPlacer.LastHit.Value.transform.GetComponentInParent<BoltEntity>();
					}
					placeConstruction.PrefabId = child.GetComponent<BoltEntity>().prefabId;
					placeConstruction.Position = child.position;
					placeConstruction.Rotation = child.rotation;
					FoundationArchitect component3 = child.GetComponent<FoundationArchitect>();
					if (component3)
					{
						placeConstruction.AboveGround = component3._aboveGround;
					}
					placeConstruction.Send();
				}
				UnityEngine.Object.Destroy(base.gameObject, 0.05f);
			}
		}
	}
}
