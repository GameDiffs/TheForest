using PathologicalGames;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class FirePoint : MonoBehaviour
{
	public Transform fireParticle;

	public Transform[] firePoints;

	public float startingFuel = 100f;

	public float fuel;

	public float fuelConsumption = 30f;

	public float fireSpread = 2f;

	public float spreadTime = 3f;

	public float randomRange = 1.2f;

	public float fallOutC = 0.05f;

	private bool fireStarted;

	private Transform[] fireC;

	private int count;

	private float fTime;

	private bool fSpread;

	private bool fellOut;

	private bool bColor;

	private Vector3 spPos = Vector3.zero;

	private void Start()
	{
		this.fuel = this.startingFuel;
	}

	private void OnDestroy()
	{
		if (this.fireStarted)
		{
			this.endFire();
		}
	}

	private void Burn()
	{
		this.startFire();
	}

	private void Update()
	{
		if (this.fireStarted)
		{
			this.fuel -= this.fuelConsumption * Time.deltaTime;
			if (Time.time - this.fTime >= this.spreadTime && !this.fSpread)
			{
				this.spreadFire();
				if (UnityEngine.Random.value > 0.95f && base.transform.root && base.transform.root.GetComponent<Renderer>() && !this.bColor)
				{
					this.bColor = true;
				}
			}
			if (!this.fellOut)
			{
				if (UnityEngine.Random.value > 1f - this.fallOutC)
				{
					this.fallOut();
				}
				this.fellOut = true;
			}
		}
		if (this.fuel <= 0f && this.fireStarted)
		{
			this.endFire();
		}
		if (this.bColor)
		{
			Color color = base.transform.root.GetComponent<Renderer>().material.color;
			color.r -= 0.01f * Time.deltaTime;
			if (color.r < 0.1f)
			{
				color.r = 0.1f;
				this.bColor = false;
			}
			color.g = color.r;
			color.b = color.r;
			base.transform.root.GetComponent<Renderer>().material.color = color;
		}
	}

	public void startFire()
	{
		if (!this.fireStarted)
		{
			this.fuel = 50f;
			this.fireStarted = true;
			Transform[] array = this.firePoints;
			for (int i = 0; i < array.Length; i++)
			{
				Transform transform = array[i];
				Transform transform2 = PoolManager.Pools["Particles"].Spawn(this.fireParticle.GetComponent<ParticleSystem>(), transform.position, Quaternion.identity).transform;
				this.fireC[this.count] = transform2;
				this.count++;
			}
			this.fTime = Time.time;
		}
	}

	public void endFire()
	{
		Transform[] array = this.fireC;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i];
			if (transform.GetComponent<ParticleSystem>())
			{
				transform.GetComponent<ParticleSystem>().enableEmission = false;
			}
			foreach (Transform transform2 in transform)
			{
				if (transform2.GetComponent<ParticleSystem>())
				{
					transform2.GetComponent<ParticleSystem>().enableEmission = false;
				}
			}
		}
		this.fireStarted = false;
		Transform[] array2 = this.firePoints;
		for (int j = 0; j < array2.Length; j++)
		{
			Transform transform3 = array2[j];
			UnityEngine.Object.Destroy(transform3.gameObject, 5f);
		}
		UnityEngine.Object.Destroy(this);
	}

	public void Awake()
	{
		this.fuel = 50f;
		this.fireC = new Transform[this.firePoints.Length];
		this.setWind();
	}

	[DebuggerHidden]
	public IEnumerator spreadFire()
	{
		FirePoint.<spreadFire>c__Iterator103 <spreadFire>c__Iterator = new FirePoint.<spreadFire>c__Iterator103();
		<spreadFire>c__Iterator.<>f__this = this;
		return <spreadFire>c__Iterator;
	}

	public void fallOut()
	{
	}

	[DebuggerHidden]
	public IEnumerator setWind()
	{
		FirePoint.<setWind>c__Iterator104 <setWind>c__Iterator = new FirePoint.<setWind>c__Iterator104();
		<setWind>c__Iterator.<>f__this = this;
		return <setWind>c__Iterator;
	}
}
