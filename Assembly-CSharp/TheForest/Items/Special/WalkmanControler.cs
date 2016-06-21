using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items.Craft;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.Special
{
	[DoNotSerializePublic]
	public class WalkmanControler : SpecialItemControlerBase
	{
		[Serializable]
		public class CassetteTrackTuple
		{
			[ItemIdPicker]
			public int _cassetteItemId;

			public int _trackNum;
		}

		public WalkmanControler.CassetteTrackTuple[] _cassetteTrackTuples;

		public float _energyPerSecond = 1f;

		[SerializeThis]
		private int _loadedCassetteTupleIndex = -1;

		[SerializeThis]
		private int _lastDayUsed = -1;

		private static WalkmanControler _instance;

		public static bool HasCassetteReady
		{
			get
			{
				return WalkmanControler._instance._loadedCassetteTupleIndex >= 0;
			}
		}

		public static int CurrentTrack
		{
			get
			{
				int loadedCassetteTupleIndex = WalkmanControler._instance._loadedCassetteTupleIndex;
				return WalkmanControler._instance._cassetteTrackTuples[loadedCassetteTupleIndex]._trackNum;
			}
		}

		protected override bool IsActive
		{
			get
			{
				return LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.LeftHand, LocalPlayer.Inventory.LastUtility._itemId);
			}
		}

		private void Awake()
		{
			WalkmanControler._instance = this;
		}

		private void OnDestroy()
		{
			if (WalkmanControler._instance == this)
			{
				WalkmanControler._instance = null;
			}
		}

		public override bool ToggleSpecial(bool enable)
		{
			if (enable && !distractionDevice.IsActive)
			{
				if (LocalPlayer.Inventory.LastUtility != this)
				{
					LocalPlayer.Inventory.StashLeftHand();
					LocalPlayer.Inventory.LastUtility = this;
				}
				base.StartCoroutine(this.PlayMusicTrack());
			}
			return true;
		}

		public static void LoadCassette(int cassetteId)
		{
			WalkmanControler._instance.LoadCassetteInternal(cassetteId);
		}

		public static void PlayCassette()
		{
			if (!PlayerSfx.MusicPlaying)
			{
				int loadedCassetteTupleIndex = WalkmanControler._instance._loadedCassetteTupleIndex;
				int trackNum = WalkmanControler._instance._cassetteTrackTuples[loadedCassetteTupleIndex]._trackNum;
				LocalPlayer.Sfx.PlayMusicTrack(trackNum);
			}
		}

		[DebuggerHidden]
		private IEnumerator PlayMusicTrack()
		{
			WalkmanControler.<PlayMusicTrack>c__Iterator16A <PlayMusicTrack>c__Iterator16A = new WalkmanControler.<PlayMusicTrack>c__Iterator16A();
			<PlayMusicTrack>c__Iterator16A.<>f__this = this;
			return <PlayMusicTrack>c__Iterator16A;
		}

		[DebuggerHidden]
		private IEnumerator RegenerationRoutine()
		{
			WalkmanControler.<RegenerationRoutine>c__Iterator16B <RegenerationRoutine>c__Iterator16B = new WalkmanControler.<RegenerationRoutine>c__Iterator16B();
			<RegenerationRoutine>c__Iterator16B.<>f__this = this;
			return <RegenerationRoutine>c__Iterator16B;
		}

		[DebuggerHidden]
		private IEnumerator enableSoundDetect()
		{
			return new WalkmanControler.<enableSoundDetect>c__Iterator16C();
		}

		protected override void OnActivating()
		{
			if (LocalPlayer.Inventory.LastUtility == this && !LocalPlayer.Animator.GetBool("drawBowBool"))
			{
				LocalPlayer.Inventory.TurnOnLastUtility();
			}
		}

		protected override void OnDeactivating()
		{
			if (LocalPlayer.Inventory.LastUtility == this)
			{
				base.StartCoroutine(this.DelayedStop(false));
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedStop(bool equipPrevious)
		{
			WalkmanControler.<DelayedStop>c__Iterator16D <DelayedStop>c__Iterator16D = new WalkmanControler.<DelayedStop>c__Iterator16D();
			<DelayedStop>c__Iterator16D.equipPrevious = equipPrevious;
			<DelayedStop>c__Iterator16D.<$>equipPrevious = equipPrevious;
			<DelayedStop>c__Iterator16D.<>f__this = this;
			return <DelayedStop>c__Iterator16D;
		}

		private void LoadCassetteInternal(int cassetteId)
		{
			LocalPlayer.Sfx.StopMusic();
			base.StopCoroutine("enableSoundDetect");
			LocalPlayer.ScriptSetup.soundDetectGo.GetComponent<SphereCollider>().radius = 0.5f;
			if (this._loadedCassetteTupleIndex >= 0)
			{
				LocalPlayer.Inventory.AddItem(this._cassetteTrackTuples[this._loadedCassetteTupleIndex]._cassetteItemId, 1, false, false, (WeaponStatUpgrade.Types)(-2));
			}
			this._loadedCassetteTupleIndex = this._cassetteTrackTuples.FindIndex((WalkmanControler.CassetteTrackTuple ctt) => ctt._cassetteItemId == cassetteId);
		}
	}
}
