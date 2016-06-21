using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio), HutongGames.PlayMaker.Tooltip("Sets the Audio Clip played by the AudioSource component on a Game Object.")]
	public class SetAudioClip : ComponentAction<AudioSource>
	{
		[CheckForComponent(typeof(AudioSource)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject with the AudioSource component.")]
		public FsmOwnerDefault gameObject;

		[ObjectType(typeof(AudioClip)), HutongGames.PlayMaker.Tooltip("The AudioClip to set.")]
		public FsmObject audioClip;

		public override void Reset()
		{
			this.gameObject = null;
			this.audioClip = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.audio.clip = (this.audioClip.Value as AudioClip);
			}
			base.Finish();
		}
	}
}
