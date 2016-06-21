using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio), HutongGames.PlayMaker.Tooltip("Stops playing the Audio Clip played by an Audio Source component on a Game Object.")]
	public class AudioStop : FsmStateAction
	{
		[CheckForComponent(typeof(AudioSource)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject with an AudioSource component.")]
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
					component.Stop();
				}
			}
			base.Finish();
		}
	}
}
