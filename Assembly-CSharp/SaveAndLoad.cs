using System;
using UnityEngine;

public class SaveAndLoad : MonoBehaviour
{
	public int[] array;

	public object arrayObject;

	public byte[] data;

	public GameObject theObject;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			this.data = LevelSerializer.SerializeLevel(false, this.theObject.GetComponent<UniqueIdentifier>().Id);
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			LevelSerializer.LoadNow(this.data, false, false);
		}
	}
}
