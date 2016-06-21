using System;
using UnityEngine;

public class amplifyDisableHook : MonoBehaviour
{
	private mutantAI ai;

	public bool skipUpdate;

	public float dist;

	private void Start()
	{
		this.ai = base.transform.GetComponentInParent<mutantAI>();
	}

	private void Update()
	{
		this.dist = this.ai.mainPlayerDist;
	}
}
