using System;
using UnityEngine;

public class Bloodify : MonoBehaviour
{
	public Renderer MyRenderer;

	public Material Bloody;

	public Material Clean;

	public bool IsBloody
	{
		get
		{
			return this.MyRenderer && this.MyRenderer.sharedMaterial == this.Bloody;
		}
	}

	public void GotBloody()
	{
		if (this.MyRenderer)
		{
			this.MyRenderer.sharedMaterial = this.Bloody;
		}
	}

	public void GotClean()
	{
		if (this.MyRenderer)
		{
			this.MyRenderer.sharedMaterial = this.Clean;
		}
	}
}
