using System;
using UnityEngine;

[Serializable]
public class GUIrescaler : MonoBehaviour
{
	private Component getTxt;

	private Component getTxtr;

	private float resX;

	private float resY;

	private float origResX;

	private float origResY;

	private float txtrX;

	private float txtrY;

	private float txtX;

	private float txtY;

	public override void Start()
	{
		this.getTxt = (GUIText)this.transform.GetComponent(typeof(GUIText));
		this.getTxtr = (GUITexture)this.transform.GetComponent(typeof(GUITexture));
		if (this.getTxtr == null && this.getTxt == null)
		{
			MonoBehaviour.print("No GUIText or GUITexture exists on: " + this.transform.gameObject.name);
		}
	}

	public override void Update()
	{
		if ((float)Screen.width != this.origResX || (float)Screen.height != this.origResY)
		{
			this.origResX = (float)Screen.width;
			this.origResY = (float)Screen.height;
			if (this.getTxt != null)
			{
				this.resX = (float)Screen.width;
				this.resY = (float)Screen.height;
				this.txtX = this.transform.localScale.x;
				this.txtY = this.transform.localScale.y;
				float y = this.transform.localScale.x * (this.resX / this.resY);
				Vector3 localScale = this.transform.localScale;
				float num = localScale.y = y;
				Vector3 vector = this.transform.localScale = localScale;
			}
			if (this.getTxtr != null)
			{
				this.resX = (float)Screen.width;
				this.resY = (float)Screen.height;
				this.txtrX = (float)this.transform.GetComponent<GUITexture>().texture.width;
				this.txtrY = (float)this.transform.GetComponent<GUITexture>().texture.height;
				float y2 = this.transform.localScale.x * (this.resX / this.resY) / (this.txtrX / this.txtrY);
				Vector3 localScale2 = this.transform.localScale;
				float num2 = localScale2.y = y2;
				Vector3 vector2 = this.transform.localScale = localScale2;
			}
		}
	}

	public override void Main()
	{
	}
}
