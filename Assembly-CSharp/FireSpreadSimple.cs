using System;
using UnityEngine;

public class FireSpreadSimple : MonoBehaviour
{
	public float fireSpread;

	private int FireDice;

	private void Start()
	{
		this.FireDice = UnityEngine.Random.Range(0, 4);
		if (this.FireDice == 2)
		{
			base.Invoke("StartFire", (float)UnityEngine.Random.Range(1, 5));
		}
	}

	private void StartFire()
	{
		this.spreadFire(base.transform.position, this.fireSpread);
	}

	private void spreadFire(Vector3 center, float radius)
	{
		Collider[] array = Physics.OverlapSphere(center, radius);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SendMessage("Burn", SendMessageOptions.RequireReceiver);
		}
	}
}
