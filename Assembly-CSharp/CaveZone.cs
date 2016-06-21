using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CaveZone : MonoBehaviour
{
	public TerrainCollider[] terrainColliders;

	private Dictionary<Collider, int> colliderTracker = new Dictionary<Collider, int>();

	private Rigidbody _rigidbody;

	private void Awake()
	{
		this._rigidbody = base.GetComponent<Rigidbody>();
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.attachedRigidbody == this._rigidbody)
		{
			return;
		}
		if (!this.colliderTracker.ContainsKey(collider))
		{
			this.colliderTracker.Add(collider, 1);
		}
		else
		{
			Dictionary<Collider, int> dictionary;
			Dictionary<Collider, int> expr_40 = dictionary = this.colliderTracker;
			int num = dictionary[collider];
			expr_40[collider] = num + 1;
		}
		if (this.colliderTracker[collider] == 1)
		{
			TerrainCollider[] array = this.terrainColliders;
			for (int i = 0; i < array.Length; i++)
			{
				TerrainCollider collider2 = array[i];
				Physics.IgnoreCollision(collider2, collider, true);
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.attachedRigidbody == this._rigidbody)
		{
			return;
		}
		if (!this.colliderTracker.ContainsKey(collider))
		{
			return;
		}
		Dictionary<Collider, int> dictionary;
		Dictionary<Collider, int> expr_2F = dictionary = this.colliderTracker;
		int num = dictionary[collider];
		expr_2F[collider] = num - 1;
		if (this.colliderTracker[collider] <= 0)
		{
			TerrainCollider[] array = this.terrainColliders;
			for (int i = 0; i < array.Length; i++)
			{
				TerrainCollider collider2 = array[i];
				Physics.IgnoreCollision(collider2, collider, false);
			}
			this.colliderTracker.Remove(collider);
		}
	}
}
