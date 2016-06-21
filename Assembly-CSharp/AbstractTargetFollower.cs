using System;
using UnityEngine;

public abstract class AbstractTargetFollower : MonoBehaviour
{
	public enum UpdateType
	{
		Auto,
		FixedUpdate,
		LateUpdate
	}

	[SerializeField]
	protected Transform target;

	[SerializeField]
	private bool autoTargetPlayer = true;

	[SerializeField]
	private AbstractTargetFollower.UpdateType updateType;

	protected virtual void Start()
	{
		if (this.autoTargetPlayer)
		{
			this.FindAndTargetPlayer();
		}
	}

	private void FixedUpdate()
	{
		if (this.updateType == AbstractTargetFollower.UpdateType.FixedUpdate || (this.updateType == AbstractTargetFollower.UpdateType.Auto && this.target.GetComponent<Rigidbody>() != null && !this.target.GetComponent<Rigidbody>().isKinematic))
		{
			if (this.autoTargetPlayer && !this.target.gameObject.activeSelf)
			{
				this.FindAndTargetPlayer();
			}
			this.FollowTarget(Time.deltaTime);
		}
	}

	private void LateUpdate()
	{
		if (this.updateType == AbstractTargetFollower.UpdateType.LateUpdate || (this.updateType == AbstractTargetFollower.UpdateType.Auto && (this.target.GetComponent<Rigidbody>() == null || this.target.GetComponent<Rigidbody>().isKinematic)))
		{
			if (this.autoTargetPlayer && !this.target.gameObject.activeSelf)
			{
				this.FindAndTargetPlayer();
			}
			this.FollowTarget(Time.deltaTime);
		}
	}

	protected abstract void FollowTarget(float deltaTime);

	public void FindAndTargetPlayer()
	{
		if (this.target == null)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
			if (gameObject)
			{
				this.target = gameObject.transform;
			}
		}
	}

	public void SetTarget(Transform newTransform)
	{
		this.target = newTransform;
	}
}
