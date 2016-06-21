using System;
using UnityEngine;

public class TurnOnAfterSave : MonoBehaviour
{
	private void OnDeserialized()
	{
		base.gameObject.SetActive(true);
	}
}
