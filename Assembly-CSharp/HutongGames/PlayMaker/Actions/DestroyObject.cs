using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Destroys a Game Object.")]
	public class DestroyObject : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to destroy.")]
		public FsmGameObject gameObject;

		[HasFloatSlider(0f, 5f), HutongGames.PlayMaker.Tooltip("Optional delay before destroying the Game Object.")]
		public FsmFloat delay;

		[HutongGames.PlayMaker.Tooltip("Detach children before destroying the Game Object.")]
		public FsmBool detachChildren;

		public override void Reset()
		{
			this.gameObject = null;
			this.delay = 0f;
		}

		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				if (this.delay.Value <= 0f)
				{
					UnityEngine.Object.Destroy(value);
				}
				else
				{
					UnityEngine.Object.Destroy(value, this.delay.Value);
				}
				if (this.detachChildren.Value)
				{
					value.transform.DetachChildren();
				}
			}
			base.Finish();
		}

		public override void OnUpdate()
		{
		}
	}
}
