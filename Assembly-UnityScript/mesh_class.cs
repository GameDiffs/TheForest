using System;
using UnityEngine;

[Serializable]
public class mesh_class
{
	[NonSerialized]
	public static bool applyToAll = true;

	public Rect area;

	public bool active;

	public bool foldout;

	public GameObject gameObject;

	public Transform transform;

	public MeshFilter meshFilter;

	public Mesh mesh;

	public MeshCollider collider;

	public mesh_class()
	{
		this.active = true;
	}
}
