using Boo.Lang.Runtime;
using System;
using System.Collections;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class ChangeAnim_rabbit : MonoBehaviour
{
	public int currentClip;

	public override void Update()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			IEnumerator enumerator = UnityRuntimeServices.GetEnumerator(this.GetComponent<Animation>());
			while (enumerator.MoveNext())
			{
				object arg_40_0;
				object expr_26 = arg_40_0 = enumerator.Current;
				if (!(expr_26 is AnimationState))
				{
					arg_40_0 = RuntimeServices.Coerce(expr_26, typeof(AnimationState));
				}
				AnimationState animationState = (AnimationState)arg_40_0;
				animationState.speed += 0.2f;
				UnityRuntimeServices.Update(enumerator, animationState);
			}
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			IEnumerator enumerator2 = UnityRuntimeServices.GetEnumerator(this.GetComponent<Animation>());
			while (enumerator2.MoveNext())
			{
				object arg_AF_0;
				object expr_95 = arg_AF_0 = enumerator2.Current;
				if (!(expr_95 is AnimationState))
				{
					arg_AF_0 = RuntimeServices.Coerce(expr_95, typeof(AnimationState));
				}
				AnimationState animationState2 = (AnimationState)arg_AF_0;
				animationState2.speed -= 0.2f;
				UnityRuntimeServices.Update(enumerator2, animationState2);
			}
		}
		if (this.currentClip == 0)
		{
			this.GetComponent<Animation>().CrossFade("walk", 0.2f);
		}
		else if (this.currentClip == 1)
		{
			this.GetComponent<Animation>().CrossFade("run", 0.2f);
		}
		else if (this.currentClip == 2)
		{
			this.GetComponent<Animation>().CrossFade("lookOut", 0.2f);
		}
		if (!this.GetComponent<Animation>().isPlaying)
		{
			this.currentClip++;
			if (this.currentClip == 3)
			{
				this.currentClip = 0;
			}
		}
	}

	public override void Main()
	{
	}
}
