using System;
using System.Collections.Generic;
using UnityEngine;

public class assignToLocations : MonoBehaviour
{
	private Vector3 pos;

	private int RandItem;

	public GameObject[] itemsGo;

	public GameObject[] locationsGo;

	private bool dont;

	private List<GameObject> locationsList = new List<GameObject>();

	private void OnDeserialized()
	{
		this.dont = true;
	}

	private void Start()
	{
		if (!this.dont)
		{
			for (int i = 0; i < this.locationsGo.Length - 1; i++)
			{
				this.locationsList.Add(this.locationsGo[i]);
			}
			GameObject[] array = this.itemsGo;
			for (int j = 0; j < array.Length; j++)
			{
				GameObject gameObject = array[j];
				if (this.locationsList.Count > 0)
				{
					if (gameObject.name == "Yacht" && BoltNetwork.isRunning)
					{
						gameObject.transform.position = this.locationsList[0].transform.position;
						this.locationsList.RemoveAt(0);
						this.locationsList.TrimExcess();
					}
					else
					{
						this.RandItem = 0;
						gameObject.transform.position = this.locationsList[this.RandItem].transform.position;
						this.locationsList.RemoveAt(this.RandItem);
						this.locationsList.TrimExcess();
						gameObject.SetActive(true);
					}
				}
			}
		}
	}
}
