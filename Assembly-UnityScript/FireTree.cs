using System;
using UnityEngine;

[Serializable]
public class FireTree : MonoBehaviour
{
	public GameObject Fire;

	public GameObject Leaves;

	public Material BurntWood;

	public Renderer WoodRender;

	public override void Burn()
	{
		if (this.Fire)
		{
			this.Fire.SetActive(true);
		}
		this.Invoke("AfterFire", (float)UnityEngine.Random.Range(15, 30));
	}

	public override void AfterFire()
	{
		if (this.Fire)
		{
			this.Fire.SetActive(false);
		}
		if (this.Leaves)
		{
			this.Leaves.SetActive(false);
		}
		if (this.WoodRender && this.BurntWood)
		{
			this.WoodRender.material = this.BurntWood;
		}
		UnityEngine.Object.Destroy(this.gameObject);
	}

	public override void Main()
	{
	}
}
