using System;
using UnityEngine;

public class KeepTrackOfDeadPeople : MonoBehaviour
{
	private void Update()
	{
		base.gameObject.GetComponent<GUIText>().text = ISeeDeadPeople.DeadPeople.ToString();
	}
}
