using System;
using UnityEngine;

public class StreamScrolling : MonoBehaviour
{
	public int materialIndex;

	public Vector2 uvAnimationRate = new Vector2(1f, 0f);

	public string textureName = "_MainTex";

	public string textureName2 = "_MainTex";

	public string textureName3 = "_MainTex";

	private Vector2 uvOffset = Vector2.zero;

	private void LateUpdate()
	{
		this.uvOffset += this.uvAnimationRate * Time.deltaTime;
		if (base.GetComponent<Renderer>().enabled)
		{
			base.GetComponent<Renderer>().materials[this.materialIndex].SetTextureOffset(this.textureName, this.uvOffset);
			base.GetComponent<Renderer>().materials[this.materialIndex].SetTextureOffset(this.textureName2, this.uvOffset);
			base.GetComponent<Renderer>().materials[this.materialIndex].SetTextureOffset(this.textureName3, this.uvOffset);
		}
	}
}
