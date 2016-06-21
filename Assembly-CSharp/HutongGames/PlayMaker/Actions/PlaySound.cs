using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio), HutongGames.PlayMaker.Tooltip("Plays an Audio Clip at a position defined by a Game Object or Vector3. If a position is defined, it takes priority over the game object. This action doesn't require an Audio Source component, but offers less control than Audio actions.")]
	public class PlaySound : FsmStateAction
	{
		public FsmOwnerDefault gameObject;

		public FsmVector3 position;

		[ObjectType(typeof(AudioClip)), RequiredField, Title("Audio Clip")]
		public FsmObject clip;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat volume = 1f;

		public override void Reset()
		{
			this.gameObject = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.clip = null;
			this.volume = 1f;
		}

		public override void OnEnter()
		{
			this.DoPlaySound();
			base.Finish();
		}

		private void DoPlaySound()
		{
			AudioClip x = this.clip.Value as AudioClip;
			if (x == null)
			{
				this.LogWarning("Missing Audio Clip!");
				return;
			}
			if (!this.position.IsNone)
			{
				AudioSource.PlayClipAtPoint(x, this.position.Value, this.volume.Value);
			}
			else
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				AudioSource.PlayClipAtPoint(x, ownerDefaultTarget.transform.position, this.volume.Value);
			}
		}
	}
}
