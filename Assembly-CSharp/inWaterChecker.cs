using System;
using System.Collections.Generic;
using UnityEngine;

public class inWaterChecker : MonoBehaviour
{
	public Animator animator;

	public animalAI ai;

	public List<Collider> waterTriggers = new List<Collider>();

	private Collider lastWaterCollider;

	private int inWaterCounter;

	public bool inWater;

	public bool swimming;

	public float waterHeight;

	private float terrainHeight;

	public float diff;

	private void FixedUpdate()
	{
		if (this.inWater)
		{
			this.terrainHeight = Terrain.activeTerrain.SampleHeight(this.animator.transform.position) + Terrain.activeTerrain.transform.position.y;
			this.diff = this.waterHeight - this.terrainHeight;
			if (this.diff > 0.5f && !this.swimming)
			{
				this.animator.SetBoolReflected("swimming", true);
				this.swimming = true;
				this.ai.swimming = true;
				this.ai.startRandomSwimSpeed();
			}
			else if (this.swimming && this.diff < 0.4f)
			{
				this.animator.SetBoolReflected("swimming", false);
				this.swimming = false;
				this.ai.swimming = false;
				this.ai.disableForceTarget();
				this.ai.disableRandomSwimSpeed();
			}
		}
		else
		{
			this.animator.SetBoolReflected("swimming", false);
			this.swimming = false;
			this.ai.swimming = false;
			this.ai.disableForceTarget();
			this.ai.disableRandomSwimSpeed();
		}
		this.inWater = false;
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Water"))
		{
			this.waterHeight = other.bounds.max.y;
			this.inWater = true;
		}
	}
}
