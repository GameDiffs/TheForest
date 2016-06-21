using System;
using UnityEngine;

public class setupPlayerAccessories : MonoBehaviour
{
	public GameObject backpackGo;

	public GameObject[] goList;

	public Transform rootTr;

	private void Start()
	{
	}

	public void doSetup()
	{
		if (this.backpackGo)
		{
			this.backpackGo.SetActive(true);
			SkinnedMeshTools.AddSkinnedMeshTo(this.backpackGo, this.rootTr);
		}
		GameObject[] array = this.goList;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			if (gameObject)
			{
				SkinnedMeshTools.AddSkinnedMeshTo(gameObject, this.rootTr);
			}
		}
	}

	private void cleanUp()
	{
		if (this.backpackGo)
		{
			UnityEngine.Object.Destroy(this.backpackGo);
		}
	}
}
