using DigitalOpus.MB.Core;
using System;
using UnityEngine;

public class MB3_TestBakeAllWithSameMaterial : MonoBehaviour
{
	public GameObject[] listOfObjsToCombineGood;

	public GameObject[] listOfObjsToCombineBad;

	private void Start()
	{
		this.testCombine();
	}

	private void testCombine()
	{
		MB3_MeshCombinerSingle mB3_MeshCombinerSingle = new MB3_MeshCombinerSingle();
		Debug.Log("About to bake 1");
		mB3_MeshCombinerSingle.AddDeleteGameObjects(this.listOfObjsToCombineGood, null, true);
		mB3_MeshCombinerSingle.Apply();
		mB3_MeshCombinerSingle.UpdateGameObjects(this.listOfObjsToCombineGood, true, true, true, true, false, false, false, false, false, false);
		mB3_MeshCombinerSingle.Apply();
		mB3_MeshCombinerSingle.AddDeleteGameObjects(null, this.listOfObjsToCombineGood, true);
		mB3_MeshCombinerSingle.Apply();
		Debug.Log("Did bake 1");
		Debug.Log("About to bake 2");
		mB3_MeshCombinerSingle.AddDeleteGameObjects(this.listOfObjsToCombineBad, null, true);
		mB3_MeshCombinerSingle.Apply();
		Debug.Log("Did bake 2");
		Debug.Log("Doing same with multi mesh combiner");
		MB3_MultiMeshCombiner mB3_MultiMeshCombiner = new MB3_MultiMeshCombiner();
		Debug.Log("About to bake 3");
		mB3_MultiMeshCombiner.AddDeleteGameObjects(this.listOfObjsToCombineGood, null, true);
		mB3_MultiMeshCombiner.Apply();
		mB3_MultiMeshCombiner.UpdateGameObjects(this.listOfObjsToCombineGood, true, true, true, true, false, false, false, false, false, false);
		mB3_MultiMeshCombiner.Apply();
		mB3_MultiMeshCombiner.AddDeleteGameObjects(null, this.listOfObjsToCombineGood, true);
		mB3_MultiMeshCombiner.Apply();
		Debug.Log("Did bake 3");
		Debug.Log("About to bake 4");
		mB3_MultiMeshCombiner.AddDeleteGameObjects(this.listOfObjsToCombineBad, null, true);
		mB3_MultiMeshCombiner.Apply();
		Debug.Log("Did bake 4");
	}
}
