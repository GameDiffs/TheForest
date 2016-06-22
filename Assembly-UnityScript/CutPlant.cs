using System;
using UnityEngine;

[Serializable]
public class CutPlant : MonoBehaviour
{
	public GameObject MyCut;

	public override void Hit()
	{
		this.CutDown();
	}

	public override void CutDown()
	{
		UnityEngine.Object.Instantiate(this.MyCut, this.transform.position, this.transform.rotation);
		UnityEngine.Object.Destroy(this.gameObject);
	}

	public override void Main()
	{
	}
}
