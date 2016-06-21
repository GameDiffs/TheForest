using System;
using UnityEngine;

public class enableGoReceiver : MonoBehaviour
{
	public GameObject[] enableGo;

	public void doEnableGo()
	{
		for (int i = 0; i < this.enableGo.Length; i++)
		{
			this.enableGo[i].SetActive(true);
		}
	}

	public void doDisableGo()
	{
		for (int i = 0; i < this.enableGo.Length; i++)
		{
			this.enableGo[i].SetActive(false);
		}
	}
}
