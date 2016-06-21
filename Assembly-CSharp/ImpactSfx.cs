using System;
using UnityEngine;

public class ImpactSfx : MonoBehaviour
{
	private const float SPEED_THRESHOLD = 2.5f;

	[Header("Impact Events")]
	public string tree;

	public string water;

	public bool impactOnTriggerEnter;

	public bool ignoreSpeedThreshold;

	private int waterLayer;

	private bool timeoutActive;

	private Rigidbody rigidBody;

	private int inWaterCount;

	private void Awake()
	{
		this.waterLayer = LayerMask.NameToLayer("Water");
		this.rigidBody = base.GetComponent<Rigidbody>();
	}

	private void Start()
	{
		FMODCommon.PreloadEvents(new string[]
		{
			this.tree,
			this.water
		});
	}

	public void OnCollisionEnter(Collision collision)
	{
		this.HandleCollision(collision.collider);
	}

	private void HandleCollision(Collider collider)
	{
		if (this.rigidBody.IsSleeping())
		{
			return;
		}
		if (collider.gameObject.CompareTag("Tree") || collider.gameObject.CompareTag("TerrainMain") || collider.gameObject.layer == 17 || collider.gameObject.layer == 20 || collider.gameObject.layer == 25 || collider.gameObject.layer == 26 || UnderfootSurfaceDetector.GetSurfaceType(collider) != UnderfootSurfaceDetector.SurfaceType.None)
		{
			this.TryPlaySound(this.tree);
		}
	}

	private void OnDisable()
	{
		this.inWaterCount = 0;
		this.timeoutActive = false;
		base.CancelInvoke("EndTimeout");
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == this.waterLayer)
		{
			this.TryPlaySound(this.water);
			this.inWaterCount++;
		}
		else if (this.impactOnTriggerEnter)
		{
			this.HandleCollision(other);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == this.waterLayer)
		{
			this.inWaterCount--;
		}
	}

	private void EndTimeout()
	{
		this.timeoutActive = false;
	}

	private bool SoundsEnabled()
	{
		return this.inWaterCount == 0 && !this.timeoutActive && (this.ignoreSpeedThreshold || this.rigidBody.velocity.magnitude > 2.5f);
	}

	private void TryPlaySound(string path)
	{
		if (this.SoundsEnabled())
		{
			FMODCommon.PlayOneshot(path, base.transform);
			this.timeoutActive = true;
			base.Invoke("EndTimeout", 1f);
		}
	}
}
