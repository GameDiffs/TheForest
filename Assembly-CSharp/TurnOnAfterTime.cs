using System;
using UnityEngine;

public class TurnOnAfterTime : MonoBehaviour
{
	public int MyWait;

	public GameObject MyGroup;

	private void Start()
	{
		base.Invoke("TurnOn", (float)this.MyWait);
	}

	private void TurnOn()
	{
		this.MyGroup.SetActive(true);
	}
}
