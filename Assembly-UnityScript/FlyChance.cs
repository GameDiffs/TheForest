using System;
using UnityEngine;

[Serializable]
public class FlyChance : MonoBehaviour
{
	private int OnChance;

	public override void Awake()
	{
		this.OnChance = UnityEngine.Random.Range(0, 8);
		if (this.OnChance != 1)
		{
			UnityEngine.Object.Destroy(this.gameObject);
		}
	}

	public override void Main()
	{
	}
}
