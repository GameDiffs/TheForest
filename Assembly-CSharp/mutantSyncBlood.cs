using System;
using UnityEngine;

public class mutantSyncBlood : MonoBehaviour
{
	private MaterialPropertyBlock sourceBloodPropertyBlock;

	public Renderer sourceRenderer;

	private Renderer thisRenderer;

	private void Awake()
	{
		this.thisRenderer = base.gameObject.GetComponent<SkinnedMeshRenderer>();
		this.sourceBloodPropertyBlock = new MaterialPropertyBlock();
	}

	private void OnEnable()
	{
		this.sourceRenderer.GetPropertyBlock(this.sourceBloodPropertyBlock);
		this.thisRenderer.SetPropertyBlock(this.sourceBloodPropertyBlock);
	}
}
