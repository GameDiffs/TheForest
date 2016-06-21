using System;
using UnityEngine;

public class SimpleTarget : MonoBehaviour
{
	private GameObject player;

	private Transform MyTransform;

	private void Start()
	{
		this.player = GameObject.Find("player");
		this.MyTransform = base.transform;
	}

	private void Update()
	{
		this.MyTransform.LookAt(this.player.transform);
	}
}
