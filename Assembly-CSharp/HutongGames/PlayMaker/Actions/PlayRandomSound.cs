using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio), HutongGames.PlayMaker.Tooltip("Plays a Random Audio Clip at a position defined by a Game Object or a Vector3. If a position is defined, it takes priority over the game object. You can set the relative weight of the clips to control how often they are selected.")]
	public class PlayRandomSound : FsmStateAction
	{
		public FsmOwnerDefault gameObject;

		public FsmVector3 position;

		[CompoundArray("Audio Clips", "Audio Clip", "Weight")]
		public AudioClip[] audioClips;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat volume = 1f;

		public override void Reset()
		{
			this.gameObject = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.audioClips = new AudioClip[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.volume = 1f;
		}

		public override void OnEnter()
		{
			this.DoPlayRandomClip();
			base.Finish();
		}

		private void DoPlayRandomClip()
		{
			if (this.audioClips.Length == 0)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				AudioClip audioClip = this.audioClips[randomWeightedIndex];
				if (audioClip != null)
				{
					if (!this.position.IsNone)
					{
						AudioSource.PlayClipAtPoint(audioClip, this.position.Value, this.volume.Value);
					}
					else
					{
						GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
						if (ownerDefaultTarget == null)
						{
							return;
						}
						AudioSource.PlayClipAtPoint(audioClip, ownerDefaultTarget.transform.position, this.volume.Value);
					}
				}
			}
		}
	}
}
