using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio), HutongGames.PlayMaker.Tooltip("Sets the Volume of the Audio Clip played by the AudioSource component on a Game Object.")]
	public class SetAudioVolume : ComponentAction<AudioSource>
	{
		[CheckForComponent(typeof(AudioSource)), RequiredField]
		public FsmOwnerDefault gameObject;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat volume;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.volume = 1f;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetAudioVolume();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetAudioVolume();
		}

		private void DoSetAudioVolume()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget) && !this.volume.IsNone)
			{
				base.audio.volume = this.volume.Value;
			}
		}
	}
}
