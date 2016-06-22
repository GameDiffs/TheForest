using System;
using UnityEngine;

[Serializable]
public class SpiderWebDynamicPlace : MonoBehaviour
{
	private Transform MyTransform;

	private Vector3 fwd;

	public GameObject Vine1;

	public GameObject Vine2;

	public GameObject Vine3;

	public GameObject Mushroom1;

	public GameObject Mushroom2;

	public GameObject Mushroom3;

	public GameObject HeavyVines;

	public GameObject Particles;

	public override void Awake()
	{
		this.MyTransform = this.transform;
		this.CheckDistance();
		this.CheckVine();
		this.CheckMushroom();
		this.CheckParticles();
	}

	public override void CheckParticles()
	{
		int num = UnityEngine.Random.Range(0, 5);
		int num2 = UnityEngine.Random.Range(0, 6);
		if (num == 3)
		{
			this.Particles.SetActive(true);
		}
		else
		{
			this.Particles.SetActive(false);
		}
	}

	public override void CheckVine()
	{
		int num = UnityEngine.Random.Range(0, 3);
		int num2 = UnityEngine.Random.Range(0, 3);
		int num3 = UnityEngine.Random.Range(0, 3);
		int num4 = UnityEngine.Random.Range(0, 9);
		if (num4 == 4)
		{
			this.HeavyVines.SetActive(true);
		}
		else if (num4 == 3)
		{
			this.HeavyVines.SetActive(false);
		}
		else
		{
			this.HeavyVines.SetActive(false);
		}
		if (num == 1)
		{
			this.Vine1.SetActive(true);
		}
		else
		{
			this.Vine1.SetActive(false);
		}
		if (num2 == 1)
		{
			this.Vine2.SetActive(true);
		}
		else
		{
			this.Vine2.SetActive(false);
		}
		if (num3 == 1)
		{
			this.Vine3.SetActive(true);
		}
		else
		{
			this.Vine3.SetActive(false);
		}
	}

	public override void CheckMushroom()
	{
		int num = UnityEngine.Random.Range(0, 3);
		int num2 = UnityEngine.Random.Range(0, 3);
		int num3 = UnityEngine.Random.Range(0, 3);
		if (num == 1)
		{
			this.Mushroom1.SetActive(true);
		}
		else
		{
			this.Mushroom1.SetActive(false);
		}
		if (num2 == 1)
		{
			this.Mushroom2.SetActive(true);
		}
		else
		{
			this.Mushroom2.SetActive(false);
		}
		if (num3 == 1)
		{
			this.Mushroom3.SetActive(true);
		}
		else
		{
			this.Mushroom3.SetActive(false);
		}
	}

	public override void CheckDistance()
	{
	}

	public override void Main()
	{
	}
}
