using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Buildings.Creation;
using TheForest.Graphics;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Player;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace TheForest.Utils
{
	public class LocalPlayer : MonoBehaviour
	{
		public Transform _transform;

		public Rigidbody _ridigbody;

		public GameObject _playerGO;

		public GameObject _playerBase;

		public Transform _headTr;

		public Transform _hipsTr;

		public PlayerInventory _inventory;

		public ReceipeBook _receipeBook;

		public GameObject _specialActions;

		public GameObject _specialItems;

		public Transform _mainCamTr;

		public Camera _mainCam;

		public Camera _inventoryCam;

		public camFollowHead _camFollowHead;

		public Animator _animator;

		public playerAnimatorControl _animControl;

		public Create _create;

		public PlayerTuts _tuts;

		public PlayerSfx _sfx;

		public PlayerStats _stats;

		public global::FirstPersonCharacter _fpc;

		public FirstPersonHeadBob _fphb;

		public SimpleMouseRotator _camRotator;

		public playerScriptSetup _scriptSetup;

		public playerTargetFunctions _targetFunctions;

		public playerHitReactions _hitReactions;

		public Buoyancy _buoyancy;

		public SimpleMouseRotator _mainRotator;

		public WaterViz _waterViz;

		public playerAiInfo _aiInfo;

		public WaterEngine _waterEngine;

		public ItemDecayMachine _itemDecayMachine;

		public SkinnedMeshRenderer _animatedBook;

		public PassengerManifest _passengerManifest;

		public GameObject _greebleRoot;

		public GreebleLayer _mudGreeble;

		public GameObject _PlayerDeadCam;

		public Blur _pauseMenuBlur;

		public Blur _pauseMenuBlurPsCam;

		public HeldItemsData _heldItemsData;

		public visRangeSetup _vis;

		public static Transform Transform;

		public static Rigidbody Ridigbody;

		public static GameObject GameObject;

		public static GameObject PlayerBase;

		public static Transform HeadTr;

		public static Transform HipsTr;

		public static PlayerInventory Inventory;

		public static ReceipeBook ReceipeBook;

		public static GameObject SpecialActions;

		public static GameObject SpecialItems;

		public static Transform MainCamTr;

		public static Camera MainCam;

		public static Camera InventoryCam;

		public static camFollowHead CamFollowHead;

		public static Animator Animator;

		public static playerAnimatorControl AnimControl;

		public static Create Create;

		public static PlayerTuts Tuts;

		public static PlayerSfx Sfx;

		public static PlayerStats Stats;

		public static global::FirstPersonCharacter FpCharacter;

		public static FirstPersonHeadBob FpHeadBob;

		public static SimpleMouseRotator CamRotator;

		public static SimpleMouseRotator MainRotator;

		public static playerScriptSetup ScriptSetup;

		public static playerTargetFunctions TargetFunctions;

		public static playerHitReactions HitReactions;

		public static Buoyancy Buoyancy;

		public static BoltEntity Entity;

		public static WaterViz WaterViz;

		public static playerAiInfo AiInfo;

		public static WaterEngine WaterEngine;

		public static ItemDecayMachine ItemDecayMachine;

		public static SkinnedMeshRenderer AnimatedBook;

		public static PassengerManifest PassengerManifest;

		public static GameObject GreebleRoot;

		public static GreebleLayer MudGreeble;

		public static GameObject PlayerDeadCam;

		public static Blur PauseMenuBlur;

		public static Blur PauseMenuBlurPsCam;

		public static HeldItemsData HeldItemsData;

		public static visRangeSetup Vis;

		private void Awake()
		{
			LocalPlayer.Transform = this._transform;
			LocalPlayer.Ridigbody = this._ridigbody;
			FMOD_StudioEventEmitter.LocalPlayerTransform = LocalPlayer.Transform;
			LocalPlayer.GameObject = this._playerGO;
			LocalPlayer.PlayerBase = this._playerBase;
			LocalPlayer.HeadTr = this._headTr;
			LocalPlayer.HipsTr = this._hipsTr;
			LocalPlayer.Inventory = this._inventory;
			LocalPlayer.ReceipeBook = this._receipeBook;
			LocalPlayer.SpecialActions = this._specialActions;
			LocalPlayer.SpecialItems = this._specialItems;
			LocalPlayer.MainCamTr = this._mainCamTr;
			LocalPlayer.MainCam = this._mainCam;
			LocalPlayer.InventoryCam = this._inventoryCam;
			LocalPlayer.CamFollowHead = this._camFollowHead;
			LocalPlayer.Animator = this._animator;
			LocalPlayer.AnimControl = this._animControl;
			LocalPlayer.Create = this._create;
			LocalPlayer.Tuts = this._tuts;
			LocalPlayer.Sfx = this._sfx;
			LocalPlayer.Stats = this._stats;
			LocalPlayer.FpCharacter = this._fpc;
			LocalPlayer.FpHeadBob = this._fphb;
			LocalPlayer.CamRotator = this._camRotator;
			LocalPlayer.MainRotator = this._mainRotator;
			LocalPlayer.ScriptSetup = this._scriptSetup;
			LocalPlayer.TargetFunctions = this._targetFunctions;
			LocalPlayer.HitReactions = this._hitReactions;
			LocalPlayer.Buoyancy = this._buoyancy;
			LocalPlayer.WaterViz = this._waterViz;
			LocalPlayer.AiInfo = this._aiInfo;
			LocalPlayer.WaterEngine = this._waterEngine;
			LocalPlayer.ItemDecayMachine = this._itemDecayMachine;
			LocalPlayer.AnimatedBook = this._animatedBook;
			LocalPlayer.PassengerManifest = this._passengerManifest;
			LocalPlayer.GreebleRoot = this._greebleRoot;
			LocalPlayer.MudGreeble = this._mudGreeble;
			LocalPlayer.PlayerDeadCam = this._PlayerDeadCam;
			LocalPlayer.PauseMenuBlur = this._pauseMenuBlur;
			LocalPlayer.PauseMenuBlurPsCam = this._pauseMenuBlurPsCam;
			LocalPlayer.HeldItemsData = this._heldItemsData;
			LocalPlayer.Vis = this._vis;
			base.StartCoroutine(this.OldSaveCompat());
		}

		private void OnDestroy()
		{
			if (LocalPlayer.Transform == base.transform)
			{
				LocalPlayer.Transform = null;
				LocalPlayer.Ridigbody = null;
				FMOD_StudioEventEmitter.LocalPlayerTransform = null;
				LocalPlayer.GameObject = null;
				LocalPlayer.PlayerBase = null;
				LocalPlayer.HeadTr = null;
				LocalPlayer.HipsTr = null;
				LocalPlayer.Inventory = null;
				LocalPlayer.ReceipeBook = null;
				LocalPlayer.SpecialActions = null;
				LocalPlayer.SpecialItems = null;
				LocalPlayer.MainCamTr = null;
				LocalPlayer.MainCam = null;
				LocalPlayer.InventoryCam = null;
				LocalPlayer.CamFollowHead = null;
				LocalPlayer.Animator = null;
				LocalPlayer.AnimControl = null;
				LocalPlayer.Create = null;
				LocalPlayer.Tuts = null;
				LocalPlayer.Sfx = null;
				LocalPlayer.Stats = null;
				LocalPlayer.FpCharacter = null;
				LocalPlayer.FpHeadBob = null;
				LocalPlayer.CamRotator = null;
				LocalPlayer.MainRotator = null;
				LocalPlayer.ScriptSetup = null;
				LocalPlayer.TargetFunctions = null;
				LocalPlayer.HitReactions = null;
				LocalPlayer.Buoyancy = null;
				LocalPlayer.WaterViz = null;
				LocalPlayer.AiInfo = null;
				LocalPlayer.WaterEngine = null;
				LocalPlayer.ItemDecayMachine = null;
				LocalPlayer.AnimatedBook = null;
				LocalPlayer.PassengerManifest = null;
				LocalPlayer.GreebleRoot = null;
				LocalPlayer.MudGreeble = null;
				LocalPlayer.PlayerDeadCam = null;
				LocalPlayer.PauseMenuBlur = null;
				LocalPlayer.PauseMenuBlurPsCam = null;
				LocalPlayer.HeldItemsData = null;
				LocalPlayer.Vis = null;
			}
		}

		[DebuggerHidden]
		private IEnumerator OldSaveCompat()
		{
			LocalPlayer.<OldSaveCompat>c__Iterator1C0 <OldSaveCompat>c__Iterator1C = new LocalPlayer.<OldSaveCompat>c__Iterator1C0();
			<OldSaveCompat>c__Iterator1C.<>f__this = this;
			return <OldSaveCompat>c__Iterator1C;
		}
	}
}
