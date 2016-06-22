using System;
using UnityEngine;

[Serializable]
public class SnakeMove : MonoBehaviour
{
	private Transform MyTransform;

	public override void Start()
	{
		this.MyTransform = this.transform;
	}

	public override void Update()
	{
		this.MyTransform.Translate(Vector3.forward * Time.deltaTime);
		float y = Terrain.activeTerrain.SampleHeight(this.MyTransform.position) + Terrain.activeTerrain.transform.position.y;
		Vector3 position = this.MyTransform.position;
		float num = position.y = y;
		Vector3 vector = this.MyTransform.position = position;
	}

	public override void Main()
	{
	}
}
