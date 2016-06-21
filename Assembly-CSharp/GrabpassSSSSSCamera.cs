using System;
using UnityEngine;

[ExecuteInEditMode]
public class GrabpassSSSSSCamera : MonoBehaviour
{
	[Range(0.01f, 10f)]
	public float ConvolutionRadius = 0.5f;

	[Range(0f, 1f)]
	public float SkinAmbientSpecular = 0.5f;

	private void ToggleKeyword(bool toggle, string keywordON, string keywordOFF)
	{
		Shader.DisableKeyword((!toggle) ? keywordON : keywordOFF);
		Shader.EnableKeyword((!toggle) ? keywordOFF : keywordON);
	}

	private void OnPreRender()
	{
		Camera current = Camera.current;
		if (current != null)
		{
			float num = 45f / current.fieldOfView;
			float num2 = num * this.ConvolutionRadius * 4f;
			Shader.SetGlobalVector("_SSSConvolutionScale", new Vector2(num2 / (float)Screen.width, num2 / (float)Screen.height));
			Shader.SetGlobalFloat("_SkinAmbientSpecular", this.SkinAmbientSpecular);
		}
	}
}
