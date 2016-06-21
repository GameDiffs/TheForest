using System;
using UnityEngine;

public class enableRandomGo : MonoBehaviour
{
	public bool hideBodyMeshes;

	public GameObject[] goList;

	private Collider col;

	private void Awake()
	{
		this.col = base.GetComponentInChildren<Collider>();
	}

	private void OnDeserialized()
	{
		this.DoRandomGo();
	}

	private void Start()
	{
		this.DoRandomGo();
	}

	private void OnEnable()
	{
		this.DoRandomGo();
	}

	private void DoRandomGo()
	{
		GameObject[] array = this.goList;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			if (gameObject)
			{
				gameObject.SetActive(false);
			}
		}
		if (!this.hideBodyMeshes)
		{
			this.goList[UnityEngine.Random.Range(0, this.goList.Length)].SetActive(true);
		}
	}

	public void hideAllGo()
	{
		this.col.enabled = false;
		GameObject[] array = this.goList;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			gameObject.SetActive(false);
		}
	}
}
