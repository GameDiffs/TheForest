using System;
using UnityEngine;

public class CreateTexture : MonoBehaviour
{
	[SerializeThis]
	private Texture2D texture;

	public Texture2D referenceTexture;

	private void Start()
	{
		if (LevelSerializer.IsDeserializing)
		{
			return;
		}
		Material material = new Material(Shader.Find("Transparent/Diffuse"));
		if (UnityEngine.Random.value < 0.8f)
		{
			this.texture = new Texture2D(2, 2);
			Color color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1f);
			Color color2 = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1f);
			this.texture.SetPixel(0, 0, color);
			this.texture.SetPixel(0, 1, color2);
			this.texture.SetPixel(1, 1, color);
			this.texture.SetPixel(1, 0, color2);
			this.texture.Apply();
		}
		else
		{
			this.texture = this.referenceTexture;
		}
		material.mainTexture = this.texture;
		base.GetComponent<Renderer>().material = material;
	}
}
