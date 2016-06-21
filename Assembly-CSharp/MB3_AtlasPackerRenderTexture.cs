using DigitalOpus.MB.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MB3_AtlasPackerRenderTexture : MonoBehaviour
{
	private MB_TextureCombinerRenderTexture fastRenderer;

	private bool _doRenderAtlas;

	public int width;

	public int height;

	public int padding;

	public bool isNormalMap;

	public bool fixOutOfBoundsUVs;

	public Rect[] rects;

	public Texture2D tex1;

	public List<MB3_TextureCombiner.MB_TexSet> textureSets;

	public int indexOfTexSetToRender;

	public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

	public Texture2D testTex;

	public Material testMat;

	public Texture2D OnRenderAtlas(MB3_TextureCombiner combiner)
	{
		this.fastRenderer = new MB_TextureCombinerRenderTexture();
		this._doRenderAtlas = true;
		Texture2D result = this.fastRenderer.DoRenderAtlas(base.gameObject, this.width, this.height, this.padding, this.rects, this.textureSets, this.indexOfTexSetToRender, this.isNormalMap, this.fixOutOfBoundsUVs, combiner, this.LOG_LEVEL);
		this._doRenderAtlas = false;
		return result;
	}

	private void OnRenderObject()
	{
		if (this._doRenderAtlas)
		{
			this.fastRenderer.OnRenderObject();
			this._doRenderAtlas = false;
		}
	}
}
