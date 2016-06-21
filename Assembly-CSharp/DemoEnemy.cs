using PathologicalGames;
using System;
using UnityEngine;

public class DemoEnemy : MonoBehaviour
{
	public int life = 100;

	public ParticleSystem explosion;

	private Color startingColor;

	private bool isDead;

	private void Awake()
	{
		this.startingColor = base.GetComponent<Renderer>().material.color;
		Targetable component = base.GetComponent<Targetable>();
		component.AddOnDetectedDelegate(new Targetable.OnDetectedDelegate(this.MakeMeBig));
		component.AddOnDetectedDelegate(new Targetable.OnDetectedDelegate(this.MakeMeGreen));
		component.AddOnNotDetectedDelegate(new Targetable.OnNotDetectedDelegate(this.MakeMeNormal));
		component.AddOnNotDetectedDelegate(new Targetable.OnNotDetectedDelegate(this.ResetColor));
		component.AddOnHitColliderDelegate(new Targetable.OnHitColliderDelegate(this.OnHit));
	}

	private void OnHit(HitEffectList effects, Target target, Collider other)
	{
		if (this.isDead)
		{
			return;
		}
		if (other != null)
		{
			Debug.Log(base.name + " was hit by collider on " + other.name);
		}
		foreach (HitEffect current in effects)
		{
			if (current.name == "Damage")
			{
				this.life -= (int)current.value;
			}
		}
		if (this.life <= 0)
		{
			this.isDead = true;
			UnityEngine.Object.Instantiate(this.explosion.gameObject, base.transform.position, base.transform.rotation);
			base.gameObject.SetActive(false);
		}
	}

	private void MakeMeGreen(TargetTracker source)
	{
		if (this.isDead)
		{
			return;
		}
		base.GetComponent<Renderer>().material.color = Color.green;
	}

	private void ResetColor(TargetTracker source)
	{
		if (this.isDead)
		{
			return;
		}
		base.GetComponent<Renderer>().material.color = this.startingColor;
	}

	private void MakeMeBig(TargetTracker source)
	{
		if (this.isDead)
		{
			return;
		}
		base.transform.localScale = new Vector3(2f, 2f, 2f);
	}

	private void MakeMeNormal(TargetTracker source)
	{
		if (this.isDead)
		{
			return;
		}
		base.transform.localScale = Vector3.one;
	}
}
