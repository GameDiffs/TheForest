using System;
using UnityEngine;

public class clscannonball : MonoBehaviour
{
	public bool vargamenabled = true;

	public clscannon varcannon;

	private void OnCollisionEnter()
	{
		if (this.vargamenabled && this.varcannon != null)
		{
			this.varcannon.metresetactor();
		}
		this.vargamenabled = false;
	}
}
