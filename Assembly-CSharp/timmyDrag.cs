using System;
using UnityEngine;

public class timmyDrag : MonoBehaviour
{
	public Transform dragPoint;

	private Transform Tr;

	private void Start()
	{
		this.Tr = base.transform;
	}

	private void Update()
	{
		this.Tr.position = this.dragPoint.position;
	}
}
