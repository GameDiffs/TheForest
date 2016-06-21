using System;
using TheForest.Utils;
using UnityEngine;

public class netPlayerVis : MonoBehaviour
{
	private Transform tr;

	public float localplayerDist;

	private void Start()
	{
		this.tr = base.transform;
	}

	private void Update()
	{
		if (LocalPlayer.Transform)
		{
			this.localplayerDist = Vector3.Distance(LocalPlayer.Transform.position, this.tr.position);
		}
	}
}
