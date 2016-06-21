using System;
using UnityEngine;

public class ShowLoading : MonoBehaviour
{
	private void OnDeserialized()
	{
		base.gameObject.SetActive(false);
	}
}
