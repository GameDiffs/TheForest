using System;
using UnityEngine;

[Serializable]
public class detailPrototype_class
{
	public bool foldout;

	public GameObject prototype;

	public Texture2D previewTexture;

	public Texture2D prototypeTexture;

	public float minWidth;

	public float maxWidth;

	public float minHeight;

	public float maxHeight;

	public float noiseSpread;

	public float bendFactor;

	public Color healthyColor;

	public Color dryColor;

	public DetailRenderMode renderMode;

	public bool usePrototypeMesh;

	public detailPrototype_class()
	{
		this.minWidth = (float)1;
		this.maxWidth = (float)2;
		this.minHeight = (float)1;
		this.maxHeight = (float)2;
		this.noiseSpread = 0.1f;
		this.healthyColor = Color.white;
		this.dryColor = new Color(0.8f, 0.76f, 0.53f);
		this.renderMode = DetailRenderMode.Grass;
	}
}
