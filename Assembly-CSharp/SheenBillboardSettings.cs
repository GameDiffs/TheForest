using System;
using UnityEngine;

[ExecuteInEditMode]
public class SheenBillboardSettings : MonoBehaviour
{
	public float Size = 0.22f;

	[Range(0f, 1f)]
	public float MaxSize = 0.8f;

	public float DistanceRatio = 10f;

	[Range(0f, 2f)]
	public float HeightOfset = 0.6f;

	public static int OverlayTextureId;

	private void Start()
	{
		SheenBillboardSettings.OverlayTextureId = Shader.PropertyToID("_OverlayTex");
	}

	private void Update()
	{
		Shader.SetGlobalVector("_SheenGlobals", new Vector4(this.Size, (1f - this.MaxSize) * 10f, this.DistanceRatio, this.HeightOfset));
	}
}
