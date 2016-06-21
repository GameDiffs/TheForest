using System;
using UnityEngine;

public class clstimemanager : MonoBehaviour
{
	private void Awake()
	{
		Time.fixedDeltaTime = 0.01f;
		Physics.defaultContactOffset = 0.01f;
		Physics.IgnoreLayerCollision(2, 0);
	}
}
