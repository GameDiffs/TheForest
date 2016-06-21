using System;
using UnityEngine;

public class AutoMoveAndRotate : MonoBehaviour
{
	[Serializable]
	public class Vector3andSpace
	{
		public Vector3 value;

		public Space space = Space.Self;
	}

	public AutoMoveAndRotate.Vector3andSpace moveUnitsPerSecond;

	public AutoMoveAndRotate.Vector3andSpace rotateDegreesPerSecond;

	public bool ignoreTimescale;

	private float lastRealTime;

	private void Start()
	{
		this.lastRealTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		float d = Time.deltaTime;
		if (this.ignoreTimescale)
		{
			d = Time.realtimeSinceStartup - this.lastRealTime;
			this.lastRealTime = Time.realtimeSinceStartup;
		}
		base.transform.Translate(this.moveUnitsPerSecond.value * d, this.moveUnitsPerSecond.space);
		base.transform.Rotate(this.rotateDegreesPerSecond.value * d, this.moveUnitsPerSecond.space);
	}
}
