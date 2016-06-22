using System;
using UnityEngine;

[Serializable]
public class DropStump : MonoBehaviour
{
	public override void Awake()
	{
		this.transform.parent = null;
	}

	public override void Main()
	{
	}
}
