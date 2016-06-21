using System;
using UnityEngine;

[AddComponentMenu("Relief Terrain/Helpers/Resolve Hole Collision"), RequireComponent(typeof(Collider))]
public class ResolveHoleCollision : MonoBehaviour
{
	public Collider[] entranceTriggers;

	public TerrainCollider[] terrainColliders;

	public float checkOffset = 1f;

	public bool StartBelowGroundSurface;

	private TerrainCollider terrainColliderForUpdate;

	private Collider _collider;

	private Rigidbody _rigidbody;

	private void Awake()
	{
		this._collider = base.GetComponent<Collider>();
		this._rigidbody = base.GetComponent<Rigidbody>();
		for (int i = 0; i < this.entranceTriggers.Length; i++)
		{
			if (this.entranceTriggers[i] != null)
			{
				this.entranceTriggers[i].isTrigger = true;
			}
		}
		if (this._rigidbody != null && this.StartBelowGroundSurface)
		{
			for (int j = 0; j < this.terrainColliders.Length; j++)
			{
				if (this.terrainColliders[j] != null && this._collider != null)
				{
					Physics.IgnoreCollision(this._collider, this.terrainColliders[j], true);
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this._collider == null)
		{
			return;
		}
		for (int i = 0; i < this.entranceTriggers.Length; i++)
		{
			if (this.entranceTriggers[i] == other)
			{
				for (int j = 0; j < this.terrainColliders.Length; j++)
				{
					Physics.IgnoreCollision(this._collider, this.terrainColliders[j], true);
				}
			}
		}
	}

	private void FixedUpdate()
	{
		if (this.terrainColliderForUpdate)
		{
			RaycastHit raycastHit = default(RaycastHit);
			if (this.terrainColliderForUpdate.Raycast(new Ray(base.transform.position + Vector3.up * this.checkOffset, Vector3.down), out raycastHit, float.PositiveInfinity))
			{
				for (int i = 0; i < this.terrainColliders.Length; i++)
				{
					Physics.IgnoreCollision(this._collider, this.terrainColliders[i], false);
				}
			}
			this.terrainColliderForUpdate = null;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (this._collider == null)
		{
			return;
		}
		for (int i = 0; i < this.entranceTriggers.Length; i++)
		{
			if (this.entranceTriggers[i] == other)
			{
				for (int j = 0; j < this.terrainColliders.Length; j++)
				{
					Physics.IgnoreCollision(this._collider, this.terrainColliders[j], false);
				}
			}
		}
	}
}
