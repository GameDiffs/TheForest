using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MB_AtlasesAndRects
{
	public Texture2D[] atlases;

	public Dictionary<Material, Rect> mat2rect_map;

	public string[] texPropertyNames;
}
