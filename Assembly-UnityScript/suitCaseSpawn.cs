using System;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class suitCaseSpawn : MonoBehaviour
{
	public GameObject[] WorldItem;

	public int Amount;

	public suitCaseSpawn()
	{
		this.Amount = 100;
	}

	public override void Start()
	{
		for (int i = 0; i < this.Amount; i++)
		{
			Vector3 position = new Vector3(UnityEngine.Random.Range(1f, 740f), (float)0, UnityEngine.Random.Range(1800f, 1280f));
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.WorldItem[UnityEngine.Random.Range(0, Extensions.get_length(this.WorldItem))], position, this.transform.rotation);
		}
	}

	public override void Main()
	{
	}
}
