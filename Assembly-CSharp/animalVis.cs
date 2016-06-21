using System;
using UnityEngine;

public class animalVis : MonoBehaviour
{
	public Mesh Lod0;

	public Mesh Lod1;

	public GameObject displacementGo;

	public MecanimEventEmitter mecanimEmitter;

	public SkinnedMeshRenderer skin;

	public animalAI ai;

	public float lodDistance = 35f;

	public float mecanimDisableDistance = 30f;

	public float displacementDisableDistance = 25f;

	public float updateRate = 0.5f;

	private float timeOffset;

	private void Start()
	{
		this.ai = base.transform.GetComponent<animalAI>();
		this.skin = base.transform.GetComponentInChildren<SkinnedMeshRenderer>();
	}

	private void Update()
	{
		if (Time.time < this.timeOffset)
		{
			return;
		}
		this.updateVis();
		this.timeOffset = Time.time + this.updateRate;
	}

	private void updateVis()
	{
		if (this.Lod1)
		{
			if (this.ai.fsmPlayerDist.Value > this.lodDistance)
			{
				this.skin.sharedMesh = this.Lod1;
			}
			else
			{
				this.skin.sharedMesh = this.Lod0;
			}
		}
		if (this.ai.fsmPlayerDist.Value > this.displacementDisableDistance)
		{
			this.displacementGo.SetActive(false);
		}
		else
		{
			this.displacementGo.SetActive(true);
		}
		if (this.ai.fsmPlayerDist.Value > this.mecanimDisableDistance)
		{
			this.mecanimEmitter.enabled = false;
		}
		else
		{
			this.mecanimEmitter.enabled = true;
		}
	}
}
