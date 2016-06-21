using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio), HutongGames.PlayMaker.Tooltip("Pauses playing the Audio Clip played by an Audio Source component on a Game Object.")]
	public class AudioPause : FsmStateAction
	{
		[CheckForComponent(typeof(AudioSource)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject with an Audio Source component.")]
		public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			this.gameObject = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
				if (component != null)
				{
					component.Pause();
				}
			}
			base.Finish();
		}
	}
}
