using System;
using UnityEngine;

public class OctreeTests : MonoBehaviour
{
	[SerializeField]
	private GameObject testObject;

	private void Start()
	{
		for (int i = 0; i < 1024; i++)
		{
			float x = (UnityEngine.Random.value - 0.5f) * 2f * 1500f;
			float y = (UnityEngine.Random.value - 0.5f) * 2f * 1500f;
			float z = (UnityEngine.Random.value - 0.5f) * 2f * 1500f;
			UnityEngine.Object.Instantiate(this.testObject, new Vector3(x, y, z), Quaternion.identity);
		}
	}

	private void OnDrawGizmos()
	{
		Octree.Instance.Draw();
	}

	private void Update()
	{
		Octree.Instance.Purge();
	}
}
