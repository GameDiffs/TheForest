using System;
using UnityEngine;

[ExecuteInEditMode]
public class ColliderMouseEvents : MonoBehaviour
{
	private void OnEnable()
	{
		UnityEngine.Object.DestroyImmediate(this);
	}
}
