using System;
using UnityEngine;

public class simpleVis : MonoBehaviour
{
	public GameObject lowMesh;

	private SkinnedMeshRenderer skinRenderer;

	public bool visible;

	private void Start()
	{
		this.skinRenderer = base.GetComponent<SkinnedMeshRenderer>();
	}

	private void Update()
	{
		if (this.skinRenderer.isVisible)
		{
			this.visible = true;
		}
		else
		{
			this.visible = false;
		}
	}
}
