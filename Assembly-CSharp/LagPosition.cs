using System;
using UnityEngine;

public class LagPosition : MonoBehaviour
{
	public Vector3 speed = new Vector3(10f, 10f, 10f);

	public bool ignoreTimeScale;

	private Transform mTrans;

	private Vector3 mRelative;

	private Vector3 mAbsolute;

	private bool mStarted;

	public void OnRepositionEnd()
	{
		this.Interpolate(1000f);
	}

	private void Interpolate(float delta)
	{
		Transform parent = this.mTrans.parent;
		if (parent != null)
		{
			Vector3 vector = parent.position + parent.rotation * this.mRelative;
			this.mAbsolute.x = Mathf.Lerp(this.mAbsolute.x, vector.x, Mathf.Clamp01(delta * this.speed.x));
			this.mAbsolute.y = Mathf.Lerp(this.mAbsolute.y, vector.y, Mathf.Clamp01(delta * this.speed.y));
			this.mAbsolute.z = Mathf.Lerp(this.mAbsolute.z, vector.z, Mathf.Clamp01(delta * this.speed.z));
			this.mTrans.position = this.mAbsolute;
		}
	}

	private void Awake()
	{
		this.mTrans = base.transform;
	}

	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.ResetPosition();
		}
	}

	private void Start()
	{
		this.mStarted = true;
		this.ResetPosition();
	}

	public void ResetPosition()
	{
		this.mAbsolute = this.mTrans.position;
		this.mRelative = this.mTrans.localPosition;
	}

	private void Update()
	{
		this.Interpolate((!this.ignoreTimeScale) ? Time.deltaTime : RealTime.deltaTime);
	}
}
