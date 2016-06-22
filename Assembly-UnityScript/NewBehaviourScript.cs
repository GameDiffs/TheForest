using System;
using UnityEngine;

[Serializable]
public class NewBehaviourScript : MonoBehaviour
{
	public float scrollSpeed;

	public Renderer rend;

	public NewBehaviourScript()
	{
		this.scrollSpeed = (float)4;
	}

	public override void Start()
	{
		this.rend = this.GetComponent<Renderer>();
	}

	public override void Update()
	{
		float y = Time.time * this.scrollSpeed;
		this.rend.material.SetTextureOffset("_MainTex", new Vector2((float)0, y));
	}

	public override void Main()
	{
	}
}
