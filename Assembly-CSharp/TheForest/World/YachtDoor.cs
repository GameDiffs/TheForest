using FMOD.Studio;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.World
{
	public class YachtDoor : MonoBehaviour
	{
		public Renderer _mainRenderer;

		public Texture _outsideClip;

		public Texture _insideClip;

		public Texture _flatOceanOutsideFlowmap;

		public Texture _flatOceanInsideFlowmap;

		public Material _flatOceanMat;

		private FMOD.Studio.EventInstance _snapshotInstance;

		private void Awake()
		{
			if (LevelSerializer.IsDeserializing)
			{
				base.StartCoroutine(this.DelayedAwake());
			}
			else if (BoltNetwork.isClient)
			{
				base.StartCoroutine(this.DelayedAwakeClient());
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				this.ToggleInside();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				this.Toggle();
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake()
		{
			YachtDoor.<DelayedAwake>c__Iterator1C9 <DelayedAwake>c__Iterator1C = new YachtDoor.<DelayedAwake>c__Iterator1C9();
			<DelayedAwake>c__Iterator1C.<>f__this = this;
			return <DelayedAwake>c__Iterator1C;
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwakeClient()
		{
			YachtDoor.<DelayedAwakeClient>c__Iterator1CA <DelayedAwakeClient>c__Iterator1CA = new YachtDoor.<DelayedAwakeClient>c__Iterator1CA();
			<DelayedAwakeClient>c__Iterator1CA.<>f__this = this;
			return <DelayedAwakeClient>c__Iterator1CA;
		}

		private void Toggle()
		{
			if (base.transform.InverseTransformPoint(LocalPlayer.Transform.position).z < 0f)
			{
				this.ToggleInside();
			}
			else
			{
				this.ToggleOutside();
			}
		}

		private void ToggleInside()
		{
			Scene.ShoreMask.clipMask = this._insideClip;
			this._flatOceanMat.SetTexture("TerrainFlowHeightmap", this._flatOceanInsideFlowmap);
			LocalPlayer.Buoyancy.enabled = false;
			if (this._snapshotInstance == null)
			{
				this._snapshotInstance = FMODCommon.PlayOneshot("snapshot:/Inside Yacht", Vector3.zero, new object[0]);
			}
		}

		private void ToggleOutside()
		{
			Scene.ShoreMask.clipMask = this._outsideClip;
			this._flatOceanMat.SetTexture("TerrainFlowHeightmap", this._flatOceanOutsideFlowmap);
			LocalPlayer.Buoyancy.enabled = true;
			if (this._snapshotInstance != null)
			{
				UnityUtil.ERRCHECK(this._snapshotInstance.stop(STOP_MODE.ALLOWFADEOUT));
				this._snapshotInstance = null;
			}
		}
	}
}
