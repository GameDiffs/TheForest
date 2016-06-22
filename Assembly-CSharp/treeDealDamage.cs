using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class treeDealDamage : MonoBehaviour
{
	private Rigidbody _rb;

	private float damage;

	private CapsuleCollider _col;

	private Vector3 currPos;

	private Vector3 nextPos;

	public GameObject treeDust;

	private bool initBool;

	private void Start()
	{
		this._rb = base.transform.GetComponent<Rigidbody>();
		this._col = base.transform.GetComponent<CapsuleCollider>();
		this.initBool = false;
		base.Invoke("doInit", 2f);
	}

	private void doInit()
	{
		this.initBool = true;
	}

	private void OnDestroy()
	{
		this._rb = null;
		this._col = null;
		this.treeDust = null;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!this.initBool)
		{
			return;
		}
		if (other.gameObject.CompareTag("playerHitDetect") || other.gameObject.CompareTag("enemyCollide") || other.gameObject.CompareTag("animalCollide"))
		{
			base.StartCoroutine("calculateDamage", other.gameObject);
		}
		if (other.gameObject.CompareTag("Tree"))
		{
			return;
		}
		if (other.gameObject.CompareTag("TerrainMain"))
		{
			float x = (this._col.bounds.center.x - Terrain.activeTerrain.transform.position.x) / Terrain.activeTerrain.terrainData.size.x;
			float y = (this._col.bounds.center.z - Terrain.activeTerrain.transform.position.z) / Terrain.activeTerrain.terrainData.size.z;
			Vector3 interpolatedNormal = Terrain.activeTerrain.terrainData.GetInterpolatedNormal(x, y);
			Quaternion rotation = Quaternion.LookRotation(Vector3.Cross(base.transform.forward, interpolatedNormal), interpolatedNormal);
			UnityEngine.Object.Instantiate(this.treeDust, this._col.bounds.center, rotation);
		}
		base.StartCoroutine("calculateDamage", other.gameObject);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!this.initBool)
		{
			return;
		}
		if (other.gameObject.CompareTag("playerHitDetect") || other.gameObject.CompareTag("enemyCollide") || other.gameObject.CompareTag("animalCollide"))
		{
			base.StartCoroutine("calculateDamage", other.gameObject);
		}
		else
		{
			string tag = other.gameObject.tag;
			if (tag != null)
			{
				if (treeDealDamage.<>f__switch$map8 == null)
				{
					treeDealDamage.<>f__switch$map8 = new Dictionary<string, int>(4)
					{
						{
							"structure",
							0
						},
						{
							"SLTier1",
							0
						},
						{
							"SLTier2",
							0
						},
						{
							"SLTier3",
							0
						}
					};
				}
				int num;
				if (treeDealDamage.<>f__switch$map8.TryGetValue(tag, out num))
				{
					if (num == 0)
					{
						base.StartCoroutine("calculateDamage", other.gameObject);
					}
				}
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator calculateDamage(GameObject target)
	{
		treeDealDamage.<calculateDamage>c__Iterator101 <calculateDamage>c__Iterator = new treeDealDamage.<calculateDamage>c__Iterator101();
		<calculateDamage>c__Iterator.target = target;
		<calculateDamage>c__Iterator.<$>target = target;
		<calculateDamage>c__Iterator.<>f__this = this;
		return <calculateDamage>c__Iterator;
	}
}
