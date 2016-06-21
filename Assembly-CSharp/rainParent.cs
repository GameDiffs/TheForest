using System;
using UnityEngine;

public class rainParent : MonoBehaviour
{
	private void Start()
	{
		base.transform.parent = null;
	}
}
