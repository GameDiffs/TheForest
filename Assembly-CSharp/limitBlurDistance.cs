using System;
using TheForest.Utils;
using UnityEngine;

public class limitBlurDistance : MonoBehaviour
{
	public AmplifyMotionObjectBase[] amplifyBase;

	private bool doAmplify;

	private Transform thisTr;

	private float playerDist;

	private void Start()
	{
		this.thisTr = base.transform;
		base.Invoke("getAmplifyObj", 0.1f);
		float value = UnityEngine.Random.value;
		base.InvokeRepeating("checkPlayerDist", value, 1f);
	}

	private void checkPlayerDist()
	{
		if (LocalPlayer.Transform != null)
		{
			this.playerDist = Vector3.Distance(this.thisTr.position, LocalPlayer.Transform.position);
		}
		if (this.playerDist > 25f && this.doAmplify)
		{
			this.disableAmplifyMotion();
		}
		else if (this.playerDist < 25f && this.doAmplify)
		{
			this.enableAmplifyMotion();
		}
	}

	private void getAmplifyObj()
	{
		if (this.amplifyBase.Length == 0)
		{
			this.amplifyBase = base.transform.parent.GetComponentsInChildren<AmplifyMotionObjectBase>();
		}
		this.doAmplify = true;
		this.disableAmplifyMotion();
	}

	private void disableAmplifyMotion()
	{
		if (this.amplifyBase != null)
		{
			for (int i = 0; i < this.amplifyBase.Length; i++)
			{
				this.amplifyBase[i].enabled = false;
			}
		}
	}

	private void enableAmplifyMotion()
	{
		if (this.amplifyBase != null)
		{
			for (int i = 0; i < this.amplifyBase.Length; i++)
			{
				this.amplifyBase[i].enabled = true;
			}
		}
	}
}
