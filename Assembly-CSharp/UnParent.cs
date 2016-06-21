using System;
using UnityEngine;

public class UnParent : MonoBehaviour
{
	private void Start()
	{
		base.transform.parent = null;
	}
}
