using System;
using UnityEngine;

[Serializable]
public class ToothChance : MonoBehaviour
{
	public GameObject Tooth;

	public override void Start()
	{
		int num = UnityEngine.Random.Range(0, 3);
		if (num == 1)
		{
			this.Tooth.SetActive(true);
		}
	}

	public override void Main()
	{
	}
}
