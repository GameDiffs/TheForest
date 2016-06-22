using System;
using UnityEngine;

[Serializable]
public class BreakWood : MonoBehaviour
{
	public GameObject MyCut;

	public GameObject MyCut2;

	public override void Hit()
	{
		this.CutDown();
	}

	public override void CutDown()
	{
		this.MyCut.SetActive(true);
		this.MyCut.transform.parent = null;
		this.MyCut2.SetActive(true);
		this.MyCut2.transform.parent = null;
		this.MyCut.GetComponent<Rigidbody>().AddForce((float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100));
		this.MyCut2.GetComponent<Rigidbody>().AddForce((float)UnityEngine.Random.Range(20, 20), (float)UnityEngine.Random.Range(20, 100), (float)UnityEngine.Random.Range(20, 100));
		UnityEngine.Object.Destroy(this.gameObject);
	}

	public override void Main()
	{
	}
}
