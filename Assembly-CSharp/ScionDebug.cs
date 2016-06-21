using System;
using System.Collections.Generic;
using UnityEngine;

public class ScionDebug
{
	private List<RenderTexture> registeredTextures = new List<RenderTexture>();

	private List<bool> forceBilinear = new List<bool>();

	private List<bool> forcePoint = new List<bool>();

	private List<bool> shouldRelease = new List<bool>();

	public void RegisterTextureForVisualization(RenderTexture texture, bool shouldRelease, bool forceBilinear = false, bool forcePoint = false)
	{
		this.registeredTextures.Add(texture);
		this.forceBilinear.Add(forceBilinear);
		this.forcePoint.Add(forcePoint);
		this.shouldRelease.Add(forcePoint);
	}

	public void VisualizeDebug(RenderTexture dest)
	{
		if (this.registeredTextures.Count != 0)
		{
			int index = this.registeredTextures.Count - 1;
			if (this.forceBilinear[index])
			{
				this.registeredTextures[index].filterMode = FilterMode.Bilinear;
			}
			if (this.forcePoint[index])
			{
				this.registeredTextures[index].filterMode = FilterMode.Point;
			}
			Graphics.Blit(this.registeredTextures[index], dest);
			for (int i = 0; i < this.shouldRelease.Count; i++)
			{
				if (this.shouldRelease[i])
				{
					RenderTexture.ReleaseTemporary(this.registeredTextures[i]);
				}
			}
			this.registeredTextures.Clear();
			this.forceBilinear.Clear();
			this.forcePoint.Clear();
			this.shouldRelease.Clear();
		}
	}
}
