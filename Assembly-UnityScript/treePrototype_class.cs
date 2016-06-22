using System;
using UnityEngine;

[Serializable]
public class treePrototype_class
{
	public GameObject prefab;

	public Texture2D texture;

	public float bendFactor;

	public bool foldout;

	public treePrototype_class()
	{
		this.bendFactor = 0.3f;
	}
}
