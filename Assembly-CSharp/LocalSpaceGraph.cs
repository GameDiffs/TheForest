using System;
using UnityEngine;

public class LocalSpaceGraph : MonoBehaviour
{
	protected Matrix4x4 originalMatrix;

	private void Start()
	{
		this.originalMatrix = base.transform.localToWorldMatrix;
	}

	public Matrix4x4 GetMatrix()
	{
		return base.transform.worldToLocalMatrix * this.originalMatrix;
	}
}
