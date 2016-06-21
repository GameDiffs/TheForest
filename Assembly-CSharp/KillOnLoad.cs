using System;
using UnityEngine;

public class KillOnLoad : MonoBehaviour
{
	private void OnDeserialized()
	{
		base.gameObject.SetActive(false);
	}
}
