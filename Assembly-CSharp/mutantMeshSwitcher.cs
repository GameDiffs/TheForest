using System;
using UnityEngine;

public class mutantMeshSwitcher : MonoBehaviour
{
	public Mesh Lod0;

	public Mesh Lod1;

	public Mesh swapMesh;

	private SkinnedMeshRenderer skin;

	private mutantScriptSetup setup;

	public bool swapMeshOnly;

	public float lod1Distance = 35f;

	private int frameOffset;

	private int frameInterval = 10;

	private void Start()
	{
		this.skin = base.transform.GetComponent<SkinnedMeshRenderer>();
		this.setup = base.transform.parent.GetComponent<mutantScriptSetup>();
		if (this.swapMeshOnly)
		{
			this.doSwapMesh();
		}
	}

	private void OnEnable()
	{
		if (this.swapMeshOnly)
		{
			this.doSwapMesh();
		}
	}

	private void Update()
	{
		if (this.swapMeshOnly)
		{
			return;
		}
		if ((Time.frameCount + this.frameOffset) % this.frameInterval != 0)
		{
			return;
		}
		this.updateVis();
	}

	private void doSwapMesh()
	{
		if (this.skin)
		{
			this.skin.sharedMesh = this.swapMesh;
		}
	}

	private void updateVis()
	{
		if (this.setup.ai.mainPlayerDist > this.lod1Distance)
		{
			this.skin.sharedMesh = this.Lod1;
		}
		else
		{
			this.skin.sharedMesh = this.Lod0;
		}
	}
}
