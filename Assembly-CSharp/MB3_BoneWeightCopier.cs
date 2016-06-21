using System;
using UnityEngine;

public class MB3_BoneWeightCopier : MonoBehaviour
{
	public GameObject inputGameObject;

	public float radius = 0.01f;

	public SkinnedMeshRenderer seamMesh;

	public string outputFolder;

	public SkinnedMeshRenderer[] targetMeshes = new SkinnedMeshRenderer[0];
}
