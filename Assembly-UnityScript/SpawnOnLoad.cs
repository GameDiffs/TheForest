using System;
using UnityEngine;

[Serializable]
public class SpawnOnLoad : MonoBehaviour
{
	public GameObject MyItem;

	public override void Start()
	{
		UnityEngine.Object.Instantiate<GameObject>(this.MyItem);
	}

	public override void Main()
	{
	}
}
