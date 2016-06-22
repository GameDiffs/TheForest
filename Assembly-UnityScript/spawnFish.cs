using System;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class spawnFish : MonoBehaviour
{
	public GameObject[] Fish;

	public int Amount;

	public spawnFish()
	{
		this.Amount = 40;
	}

	public override void Start()
	{
		for (int i = 0; i < this.Amount; i++)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.Fish[UnityEngine.Random.Range(0, Extensions.get_length(this.Fish))], this.transform.position, this.transform.rotation);
		}
	}

	public override void Main()
	{
	}
}
