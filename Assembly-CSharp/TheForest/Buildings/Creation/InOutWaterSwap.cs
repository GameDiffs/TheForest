using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	public class InOutWaterSwap : MonoBehaviour
	{
		public enum RayCastOrigins
		{
			Player,
			Placer
		}

		public LayerMask _groundLayers;

		public LayerMask _waterLayers;

		public InOutWaterSwap.RayCastOrigins _origin;

		public float _raycastDistance = 3f;

		public GameObject _outWaterGo;

		public GameObject _inWaterGo;

		public bool _outWaterHasCustomPlace;

		public bool _inWaterHasCustomPlace;

		public bool _initializeOutWater;

		public bool _initializeInWater;

		private Renderer _outWaterRenderer;

		private Renderer _inWaterRenderer;

		[DebuggerHidden]
		private IEnumerator Start()
		{
			InOutWaterSwap.<Start>c__Iterator13B <Start>c__Iterator13B = new InOutWaterSwap.<Start>c__Iterator13B();
			<Start>c__Iterator13B.<>f__this = this;
			return <Start>c__Iterator13B;
		}

		private void Update()
		{
			bool flag = LocalPlayer.FpCharacter.Diving || LocalPlayer.FpCharacter.swimming;
			RaycastHit raycastHit = default(RaycastHit);
			if (flag || Physics.Raycast(((this._origin != InOutWaterSwap.RayCastOrigins.Player) ? LocalPlayer.Create.BuildingPlacer.transform.position : LocalPlayer.MainCamTr.position) + new Vector3(0f, this._raycastDistance / 2f), Vector3.down, out raycastHit, (!this._inWaterGo.activeSelf) ? this._raycastDistance : (this._raycastDistance * 2f), this._groundLayers | this._waterLayers))
			{
				bool flag2 = flag || (1 << raycastHit.collider.gameObject.layer & this._waterLayers) != 0;
				if (flag2)
				{
					this._inWaterGo.SetActive(true);
					this._outWaterGo.SetActive(false);
					if (this._outWaterHasCustomPlace)
					{
						LocalPlayer.Create.Grabber.ClosePlace();
					}
					else
					{
						LocalPlayer.Create.Grabber.ShowPlace();
					}
					LocalPlayer.Create.BuildingPlacer.SetClear();
					LocalPlayer.Create.BuildingPlacer.SetRenderer(this._outWaterRenderer);
					Scene.HudGui.LockPositionIcon.SetActive(false);
					Scene.HudGui.UnlockPositionIcon.SetActive(false);
					Scene.HudGui.ToggleAutoFillIcon.SetActive(false);
					Scene.HudGui.ToggleManualPlacementIcon.SetActive(false);
				}
				else if (this._inWaterGo.activeSelf || !this._outWaterGo.activeSelf)
				{
					this._inWaterGo.SetActive(false);
					this._outWaterGo.SetActive(true);
					if (this._inWaterHasCustomPlace)
					{
						LocalPlayer.Create.Grabber.ClosePlace();
					}
					else
					{
						LocalPlayer.Create.Grabber.ShowPlace();
					}
					LocalPlayer.Create.BuildingPlacer.SetRenderer(this._inWaterRenderer);
					Scene.HudGui.LockPositionIcon.SetActive(false);
					Scene.HudGui.UnlockPositionIcon.SetActive(false);
					Scene.HudGui.ToggleAutoFillIcon.SetActive(false);
					Scene.HudGui.ToggleManualPlacementIcon.SetActive(false);
				}
			}
			else if (!this._outWaterGo.activeSelf)
			{
				this._inWaterGo.SetActive(false);
				this._outWaterGo.SetActive(true);
				if (this._inWaterHasCustomPlace)
				{
					LocalPlayer.Create.Grabber.ClosePlace();
				}
				else
				{
					LocalPlayer.Create.Grabber.ShowPlace();
				}
				LocalPlayer.Create.BuildingPlacer.SetRenderer(this._inWaterRenderer);
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
			if (this._inWaterGo.activeSelf)
			{
				child = this._inWaterGo.transform.GetChild(0);
			}
			else
			{
				child = this._outWaterGo.transform.GetChild(0);
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
				if ((this._inWaterGo.activeSelf && this._initializeOutWater) || (this._outWaterGo.activeSelf && this._initializeInWater))
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
