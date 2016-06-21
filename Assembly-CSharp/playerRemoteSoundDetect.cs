using System;
using UnityEngine;

public class playerRemoteSoundDetect : MonoBehaviour
{
	private Transform tr;

	public SphereCollider soundCollider;

	public float maxSpeed;

	private float overallSpeed;

	private Vector3 prevPos;

	private void Start()
	{
		this.tr = base.transform.parent;
		this.prevPos = this.tr.position;
	}

	private void FixedUpdate()
	{
		this.overallSpeed = (this.tr.position - this.prevPos).magnitude / Time.deltaTime;
		this.prevPos = this.tr.position;
		this.overallSpeed /= this.maxSpeed;
		if (this.overallSpeed > 1.5f)
		{
			base.Invoke("setRunSoundRange", 0.4f);
		}
		else if (this.overallSpeed > 0.9f)
		{
			base.Invoke("setWalkSoundRange", 0.4f);
		}
		else
		{
			this.soundCollider.radius = 1f;
		}
	}

	private void setRunSoundRange()
	{
		if (this.overallSpeed > 1.5f)
		{
			this.soundCollider.radius = 42f;
		}
	}

	private void setWalkSoundRange()
	{
		if (this.overallSpeed > 0.9f && this.overallSpeed < 1.5f)
		{
			this.soundCollider.radius = 21f;
		}
	}

	private void setIdleSoundRange()
	{
		this.soundCollider.radius = 1f;
	}
}
