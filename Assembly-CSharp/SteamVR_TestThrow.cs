using System;
using UnityEngine;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class SteamVR_TestThrow : MonoBehaviour
{
	public GameObject prefab;

	public Rigidbody attachPoint;

	private SteamVR_TrackedObject trackedObj;

	private FixedJoint joint;

	private void Awake()
	{
		this.trackedObj = base.GetComponent<SteamVR_TrackedObject>();
	}

	private void FixedUpdate()
	{
		SteamVR_Controller.Device device = SteamVR_Controller.Input((int)this.trackedObj.index);
		if (this.joint == null && device.GetTouchDown(8589934592uL))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab);
			gameObject.transform.position = this.attachPoint.transform.position;
			this.joint = gameObject.AddComponent<FixedJoint>();
			this.joint.connectedBody = this.attachPoint;
		}
		else if (this.joint != null && device.GetTouchUp(8589934592uL))
		{
			GameObject gameObject2 = this.joint.gameObject;
			Rigidbody component = gameObject2.GetComponent<Rigidbody>();
			UnityEngine.Object.DestroyImmediate(this.joint);
			this.joint = null;
			UnityEngine.Object.Destroy(gameObject2, 15f);
			Transform transform = (!this.trackedObj.origin) ? this.trackedObj.transform.parent : this.trackedObj.origin;
			if (transform != null)
			{
				component.velocity = transform.TransformVector(device.velocity);
				component.angularVelocity = transform.TransformVector(device.angularVelocity);
			}
			else
			{
				component.velocity = device.velocity;
				component.angularVelocity = device.angularVelocity;
			}
			component.maxAngularVelocity = component.angularVelocity.magnitude;
		}
	}
}
