using System;
using UnityEngine;

[Serializable]
public class PlayerHeadControl : MonoBehaviour
{
	private Transform MyTransform;

	private Transform Head;

	public override void Awake()
	{
		this.Head = GameObject.Find("char_Head1").transform;
		this.MyTransform = this.transform;
	}

	public override void Start()
	{
	}

	public override void LateUpdate()
	{
		this.MyTransform.position = this.Head.position;
	}

	public override void Main()
	{
	}
}
