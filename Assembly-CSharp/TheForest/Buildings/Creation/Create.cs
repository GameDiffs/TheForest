using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic]
	public class Create : MonoBehaviour
	{
		[DoNotSerializePublic]
		[Serializable]
		public class BuildingBlueprint
		{
			public Create.BuildingTypes _type;

			public Create.PlacerDistance _placerDistance;

			public GameObject _ghostPrefab;

			public GameObject _ghostPrefabMP;

			public GameObject _builtPrefab;

			public bool _allowInTree;

			public bool _allowInTreeAtFloorLevel;

			public bool _isPlateform;

			public bool _isWallPiece;

			public bool _isStairPiece;

			public bool _allowFoundation;

			public bool _airBorne;

			public bool _showAnchors;

			public bool _isDynamic;

			public bool _allowParentingToDynamic;

			public bool _waterborne;

			public bool _waterborneExclusive;
		}

		public enum BuildingTypes
		{
			None,
			Cage = 10,
			EffigyHead = 20,
			EffigyLarge = 30,
			EffigyRain = 40,
			EffigySmall = 50,
			EffigyEx = 55,
			Fire = 60,
			FireParallel = 63,
			FireReflector = 66,
			FireRockPit = 70,
			FireStand = 80,
			Roof,
			Floor,
			Stairs,
			Dock,
			Foundation,
			FloorHole = 87,
			RaftEx,
			Garden = 90,
			HouseBoat = 100,
			LeafShelter = 110,
			LogCabin = 120,
			LogCabinMed = 130,
			LogHolder = 140,
			LogSled = 150,
			Platform = 160,
			RockSidePlatform,
			PlatformBridge,
			TreePlatform = 165,
			Raft = 170,
			RockHolder = 180,
			Shelter = 190,
			Staircase = 200,
			ClimbingRope = 205,
			StickHolder = 210,
			StickMarker = 220,
			TempShelter = 225,
			TrapDeadfall = 230,
			TrapPole = 235,
			TrapRope = 240,
			TrapSimple = 245,
			TrapSpikedWall = 250,
			TreeHouse = 260,
			Wall = 270,
			WallEx,
			StickFence,
			RockFence,
			BoneFence,
			WallDefensive = 280,
			WallDefensiveReinforcement,
			WallDoorway = 290,
			WallWindow = 300,
			WorkBench = 310,
			ItemStash = 315,
			WeaponRack,
			DryingRack = 320,
			BonFire = 330,
			TreeHouseChatel = 340,
			WalkWay = 350,
			RabbitTrap = 360,
			FishTrap = 365,
			Gazeebo = 370,
			DefensiveSpikes = 380,
			MedicineCabinet = 390,
			ExplosiveHolder = 400,
			ArrowBasket = 402,
			BoneBasket = 404,
			SnackHolder = 410,
			TrapTripWireExplosive = 420,
			TrapTripWireMolotov,
			TrapLeafPile = 425,
			SkullLight = 430,
			CeilingSkullLight,
			Bed = 440,
			Target = 450,
			PedestalTable = 460,
			WallItemCache = 500,
			FloorItemCache,
			WaterCollector = 600,
			TreeSapCollector = 610,
			Decoration_deerSkin = 2000,
			Decoration_rabbitSkin,
			Decoration_skull = 2100,
			Decoration_Trophy = 2200,
			Decoration_WallWeaponHolder = 2300,
			Decoration_WallPlantPot = 2400,
			TrapSwingingRock = 251,
			RockThrower = 2500
		}

		public enum PlacerDistance
		{
			Close,
			Medium,
			Far
		}

		public KeepAboveTerrain[] BuildingPlacerCloseMedFar;

		public WallClick Grabber;

		public Transform TargetTree;

		public GameObject SurvivalBook;

		public PlayerInventory Inventory;

		[NameFromProperty("_type")]
		public List<Create.BuildingBlueprint> _blueprints;

		[HideInInspector]
		public bool CreateMode;

		private KeepAboveTerrain _buildingPlacer;

		private Camera GuiCam;

		private bool ShouldOpenBook;

		private bool ToolsShown;

		private bool ShownPlace;

		private float enterCreateModeCoolDown;

		private Create.BuildingBlueprint _currentBlueprint;

		private GameObject _currentGhost;

		public Create.BuildingBlueprint CurrentBlueprint
		{
			get
			{
				return this._currentBlueprint;
			}
		}

		public GameObject CurrentGhost
		{
			get
			{
				return this._currentGhost;
			}
		}

		public KeepAboveTerrain BuildingPlacer
		{
			get
			{
				return this._buildingPlacer;
			}
		}

		private void Awake()
		{
			this.GuiCam = Scene.HudGui.GuiCamC;
		}

		private void Update()
		{
			if ((LocalPlayer.AnimControl.onRope || LocalPlayer.AnimControl.useRootMotion || LocalPlayer.AnimControl.onRockThrower) && this.Inventory.CurrentView == PlayerInventory.PlayerViews.Book)
			{
				this.ShouldOpenBook = false;
				this.CloseTheBook();
				return;
			}
			if (TheForest.Utils.Input.GetButtonDown("Esc") && this.Inventory.CurrentView == PlayerInventory.PlayerViews.Book)
			{
				this.CloseTheBook();
			}
			if (this.ShouldOpenBook)
			{
				this.OpenBook();
			}
			if (this.CreateMode)
			{
				if (!this.ShownPlace)
				{
					if (TheForest.Utils.Input.IsGamePad)
					{
						LocalPlayer.FpCharacter.CanJump = false;
					}
					this.Grabber.ShowPlace();
					this.ShownPlace = true;
				}
				bool button = TheForest.Utils.Input.GetButton("Batch");
				if (Scene.HudGui.BatchPlaceIcon.activeSelf != button)
				{
					Scene.HudGui.BatchPlaceIcon.SetActive(button);
				}
				if (TheForest.Utils.Input.GetButtonDown("Esc"))
				{
					this.CancelPlace();
				}
				else if (this._buildingPlacer.Clear && TheForest.Utils.Input.GetButtonDown("Build"))
				{
					this.PlaceGhost(button);
				}
			}
		}

		public void CloseBuildMode()
		{
			this.CreateMode = false;
		}

		public void CancelPlace()
		{
			if (this._currentGhost)
			{
				LocalPlayer.Sfx.PlayWhoosh();
				UnityEngine.Object.Destroy(this._currentGhost);
			}
			this.ClearReferences(true);
			this.CreateMode = false;
		}

		public void PlaceGhost(bool chain = false)
		{
			base.StartCoroutine(this.PlaceGhostRoutine(chain));
		}

		[DebuggerHidden]
		private IEnumerator PlaceGhostRoutine(bool chain)
		{
			Create.<PlaceGhostRoutine>c__Iterator132 <PlaceGhostRoutine>c__Iterator = new Create.<PlaceGhostRoutine>c__Iterator132();
			<PlaceGhostRoutine>c__Iterator.chain = chain;
			<PlaceGhostRoutine>c__Iterator.<$>chain = chain;
			<PlaceGhostRoutine>c__Iterator.<>f__this = this;
			return <PlaceGhostRoutine>c__Iterator;
		}

		public BoltEntity GetParentEntity(GameObject ghost)
		{
			BoltEntity boltEntity = null;
			SingleAnchorStructure component = ghost.GetComponent<SingleAnchorStructure>();
			if (component && component.Anchor1)
			{
				component.enabled = false;
				boltEntity = component.Anchor1.GetComponentInParent<BoltEntity>();
			}
			if (this._buildingPlacer.ForcedParent)
			{
				boltEntity = this._buildingPlacer.ForcedParent.GetComponentInParent<BoltEntity>();
				this._buildingPlacer.ForcedParent = null;
			}
			else if (this._buildingPlacer.LastHit.HasValue && !this._currentBlueprint._isDynamic && !boltEntity)
			{
				boltEntity = this._buildingPlacer.LastHit.Value.transform.GetComponentInParent<BoltEntity>();
			}
			if (!boltEntity)
			{
				boltEntity = null;
			}
			else
			{
				DynamicBuilding component2 = boltEntity.GetComponent<DynamicBuilding>();
				if (component2 && (!this._currentBlueprint._allowParentingToDynamic || !component2._allowParenting))
				{
					boltEntity = null;
				}
			}
			return boltEntity;
		}

		public CoopConstructionExToken GetCoopConstructionExToken(CoopConstructionEx coopEx, BoltEntity parentEntity)
		{
			CoopConstructionExToken coopConstructionExToken = new CoopConstructionExToken();
			coopConstructionExToken.Architects = new CoopConstructionExToken.ArchitectData[coopEx.Architects.Length];
			for (int i = 0; i < coopEx.Architects.Length; i++)
			{
				coopConstructionExToken.Parent = parentEntity;
				coopConstructionExToken.Architects[i].PointsCount = (coopEx.Architects[i] as ICoopStructure).MultiPointsCount;
				coopConstructionExToken.Architects[i].PointsPositions = (coopEx.Architects[i] as ICoopStructure).MultiPointsPositions.ToArray();
				coopConstructionExToken.Architects[i].CustomToken = (coopEx.Architects[i] as ICoopStructure).CustomToken;
				if (coopEx.Architects[i] is FoundationArchitect)
				{
					coopConstructionExToken.Architects[i].AboveGround = ((FoundationArchitect)coopEx.Architects[i])._aboveGround;
				}
				if (coopEx.Architects[i] is RoofArchitect && (coopEx.Architects[i] as RoofArchitect).CurrentSupport != null)
				{
					coopConstructionExToken.Architects[i].Support = ((coopEx.Architects[i] as RoofArchitect).CurrentSupport as MonoBehaviour).GetComponent<BoltEntity>();
				}
				if (coopEx.Architects[i] is FloorArchitect && (coopEx.Architects[i] as FloorArchitect).CurrentSupport != null)
				{
					coopConstructionExToken.Architects[i].Support = ((coopEx.Architects[i] as FloorArchitect).CurrentSupport as MonoBehaviour).GetComponent<BoltEntity>();
				}
			}
			return coopConstructionExToken;
		}

		public void RefreshGrabber()
		{
			base.StartCoroutine(this.RefreshGrabberRoutine());
		}

		[DebuggerHidden]
		private IEnumerator RefreshGrabberRoutine()
		{
			Create.<RefreshGrabberRoutine>c__Iterator133 <RefreshGrabberRoutine>c__Iterator = new Create.<RefreshGrabberRoutine>c__Iterator133();
			<RefreshGrabberRoutine>c__Iterator.<>f__this = this;
			return <RefreshGrabberRoutine>c__Iterator;
		}

		private void ClearReferences(bool equipPrevious)
		{
			if (this._currentGhost)
			{
				if (this._currentGhost.transform.parent == this._buildingPlacer.transform)
				{
					this._currentGhost.transform.parent = null;
				}
				this._currentGhost = null;
			}
			this._currentBlueprint = null;
			if (this._buildingPlacer)
			{
				this._buildingPlacer.MyRender = null;
				this._buildingPlacer.gameObject.SetActive(false);
			}
			this.Grabber.ClosePlace();
			Scene.HudGui.PlaceWallIcon.SetActive(false);
			Scene.HudGui.BatchPlaceIcon.SetActive(false);
			Scene.HudGui.ToggleAutoFillIcon.SetActive(false);
			Scene.HudGui.ToggleManualPlacementIcon.SetActive(false);
			Scene.HudGui.AngleAndDistanceGizmo.gameObject.SetActive(false);
			Scene.HudGui.AngleAndDistanceGizmoWall.gameObject.SetActive(false);
			Scene.HudGui.SnappingGridGizmo.gameObject.SetActive(false);
			if (equipPrevious)
			{
				this.Inventory.EquipPreviousUtility();
				this.Inventory.EquipPreviousWeaponDelayed();
			}
			LocalPlayer.FpCharacter.CanJump = true;
		}

		public void OpenBook()
		{
			if (this.enterCreateModeCoolDown > Time.time)
			{
				return;
			}
			if (LocalPlayer.AnimControl.onRope || LocalPlayer.AnimControl.useRootMotion || LocalPlayer.AnimControl.onRockThrower)
			{
				this.ShouldOpenBook = false;
				this.CloseTheBook();
				return;
			}
			if (LocalPlayer.FpCharacter.Grounded)
			{
				if (LocalPlayer.AnimControl.USE_NEW_BOOK)
				{
					this.ShouldOpenBook = false;
					this.GuiCam.enabled = false;
					this.CreateMode = false;
					if (this._currentGhost != null)
					{
						UnityEngine.Object.Destroy(this._currentGhost);
						this.ClearReferences(false);
					}
					else
					{
						this.Inventory.HideAllEquiped(true);
					}
					this.ShownPlace = false;
					LocalPlayer.AnimControl.cancelAnimatorActions();
					this.Inventory.Close();
					this.Inventory.CurrentView = PlayerInventory.PlayerViews.Book;
					LocalPlayer.Animator.SetBoolReflected("bookHeld", true);
					LocalPlayer.Tuts.HideStoryClueTut();
					LocalPlayer.FpCharacter.CanJump = false;
				}
				else
				{
					this.ShouldOpenBook = false;
					this.Inventory.HideAllEquiped(true);
					this.GuiCam.enabled = false;
					this.CreateMode = false;
					if (this._currentGhost != null)
					{
						UnityEngine.Object.Destroy(this._currentGhost);
						this.ClearReferences(false);
					}
					else
					{
						LocalPlayer.Tuts.HideStoryClueTut();
					}
					this.ShownPlace = false;
					this.SurvivalBook.SetActive(true);
					this.SurvivalBook.SendMessage("CheckPage");
					this.Inventory.Close();
					this.Inventory.CurrentView = PlayerInventory.PlayerViews.Book;
					LocalPlayer.FpCharacter.LockView(false);
				}
			}
			else
			{
				this.ShouldOpenBook = true;
			}
		}

		public void CloseBookForInventory()
		{
			this.CloseTheBook();
		}

		public void CloseTheBook()
		{
			if (this.Inventory.CurrentView == PlayerInventory.PlayerViews.Book)
			{
				if (!this.CreateMode && !LocalPlayer.AnimControl.upsideDown)
				{
					base.Invoke("showEquipped", 0.65f);
				}
				if (LocalPlayer.AnimControl.USE_NEW_BOOK)
				{
					this.GuiCam.enabled = true;
					LocalPlayer.FpCharacter.UnLockView();
					LocalPlayer.FpCharacter.CanJump = true;
					this.Inventory.CurrentView = PlayerInventory.PlayerViews.World;
					if (this.CreateMode)
					{
						LocalPlayer.Animator.SetInteger("bookCloseInt", 1);
					}
					else
					{
						LocalPlayer.Animator.SetInteger("bookCloseInt", 0);
					}
					LocalPlayer.Animator.SetBoolReflected("bookHeld", false);
				}
				else
				{
					this.GuiCam.enabled = true;
					this.SurvivalBook.SendMessage("CloseBook", SendMessageOptions.DontRequireReceiver);
					this.SurvivalBook.SetActive(false);
					this.Inventory.CurrentView = PlayerInventory.PlayerViews.World;
				}
				this.ShouldOpenBook = false;
			}
		}

		private void showEquipped()
		{
			this.Inventory.ShowAllEquiped();
		}

		public void CreateBuilding(Create.BuildingTypes type)
		{
			this._currentBlueprint = this._blueprints.Find((Create.BuildingBlueprint bp) => bp._type == type);
			if (this._currentBlueprint == null)
			{
				UnityEngine.Debug.LogError("Building blueprint not found on Create script for " + type);
			}
			else
			{
				this._buildingPlacer = this.BuildingPlacerCloseMedFar[(int)this._currentBlueprint._placerDistance];
				this._buildingPlacer.gameObject.SetActive(true);
				this._buildingPlacer.transform.rotation = Quaternion.identity;
				if (!BoltNetwork.isRunning || !this._currentBlueprint._ghostPrefabMP)
				{
					this._currentGhost = UnityEngine.Object.Instantiate<GameObject>(this._currentBlueprint._ghostPrefab);
				}
				else
				{
					this._currentGhost = UnityEngine.Object.Instantiate<GameObject>(this._currentBlueprint._ghostPrefabMP);
				}
				this._currentGhost.transform.parent = this._buildingPlacer.transform;
				this._currentGhost.transform.localRotation = Quaternion.identity;
				GameObject gameObject = this._currentGhost.transform.FindChild("Trigger").gameObject;
				gameObject.SetActive(false);
				BoxCollider boxCollider = (BoxCollider)this._buildingPlacer.GetComponent<Collider>();
				Bounds bounds = default(Bounds);
				GhostRendererSelector[] componentsInChildren = this._currentGhost.GetComponentsInChildren<GhostRendererSelector>();
				if (componentsInChildren.Length > 0)
				{
					this._currentGhost.transform.localRotation = Quaternion.Inverse(componentsInChildren[0].transform.rotation);
					this._buildingPlacer.MyRender = componentsInChildren[0].GetComponent<Renderer>();
					bounds = componentsInChildren[0].GetComponent<Renderer>().bounds;
					for (int i = 1; i < componentsInChildren.Length; i++)
					{
						bounds.Encapsulate(componentsInChildren[i].GetComponent<Renderer>().bounds);
					}
				}
				else
				{
					this._buildingPlacer.MyRender = this._currentGhost.GetComponent<Renderer>();
					GhostColliderSelector[] componentsInChildren2 = this._currentGhost.GetComponentsInChildren<GhostColliderSelector>();
					if (componentsInChildren2.Length > 0)
					{
						this._currentGhost.transform.localRotation = Quaternion.Inverse(componentsInChildren2[0].transform.rotation);
						bounds = componentsInChildren2[0].GetComponent<Collider>().bounds;
						for (int j = 1; j < componentsInChildren2.Length; j++)
						{
							bounds.Encapsulate(componentsInChildren2[j].GetComponent<Collider>().bounds);
						}
					}
					else if (this._currentGhost.GetComponent<Renderer>())
					{
						bounds = this._currentGhost.GetComponent<Renderer>().bounds;
					}
				}
				boxCollider.size = bounds.size;
				boxCollider.center = new Vector3(0f, bounds.extents.y * 1.05f, 0f);
				this._currentGhost.transform.localPosition = this._currentGhost.transform.position - bounds.center + boxCollider.center;
				this._buildingPlacer.TreeStructure = this._currentBlueprint._allowInTree;
				this._buildingPlacer.AllowFoundation = this._currentBlueprint._allowFoundation;
				this._buildingPlacer.Airborne = this._currentBlueprint._airBorne;
				this._buildingPlacer.ShowAnchorArea.SetActive(this._currentBlueprint._showAnchors);
				if (this._currentBlueprint._waterborne)
				{
					this._buildingPlacer.SetWaterborne(this._currentBlueprint._waterborneExclusive);
				}
				this.enterCreateModeCoolDown = Time.time + 0.7f;
				this.CreateMode = true;
				this.ShouldOpenBook = false;
				this.Inventory.EquipPreviousUtility();
			}
		}

		private void ClosedCreate()
		{
			LocalPlayer.FpCharacter.UnLockView();
		}
	}
}
