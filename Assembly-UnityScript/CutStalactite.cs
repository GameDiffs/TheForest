using System;
using UnityEngine;

[Serializable]
public class CutStalactite : MonoBehaviour
{
	public GameObject MyCut;

	public override void Hit()
	{
		this.CutDown();
	}

	public override void CutDown()
	{
		UnityEngine.Object.Instantiate(this.MyCut, this.transform.position, this.transform.rotation);
		this.MyCut.transform.localScale = this.transform.localScale;
		UnityEngine.Object.Destroy(this.gameObject);
	}

	public override void Main()
	{
	}
}
