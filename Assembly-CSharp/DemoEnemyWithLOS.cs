using PathologicalGames;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class DemoEnemyWithLOS : MonoBehaviour
{
	public int life = 100;

	public ParticleSystem explosion;

	private Color startingColor;

	private bool isDead;

	private void Awake()
	{
		this.startingColor = base.GetComponent<Renderer>().material.color;
		Targetable component = base.GetComponent<Targetable>();
		component.AddOnDetectedDelegate(new Targetable.OnDetectedDelegate(this.OnDetected));
		component.AddOnNotDetectedDelegate(new Targetable.OnNotDetectedDelegate(this.OnNotDetected));
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
			UnityEngine.Debug.Log(base.name + " was hit by collider on " + other.name);
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

	private void OnDetected(TargetTracker source)
	{
		base.StartCoroutine(this.UpdateStartWhileDetected(source));
	}

	private void OnNotDetected(TargetTracker source)
	{
		base.StopAllCoroutines();
		this.ResetStates();
	}

	private void ResetStates()
	{
		base.transform.localScale = Vector3.one;
		base.GetComponent<Renderer>().material.color = this.startingColor;
	}

	[DebuggerHidden]
	private IEnumerator UpdateStartWhileDetected(TargetTracker source)
	{
		DemoEnemyWithLOS.<UpdateStartWhileDetected>c__Iterator1E3 <UpdateStartWhileDetected>c__Iterator1E = new DemoEnemyWithLOS.<UpdateStartWhileDetected>c__Iterator1E3();
		<UpdateStartWhileDetected>c__Iterator1E.source = source;
		<UpdateStartWhileDetected>c__Iterator1E.<$>source = source;
		<UpdateStartWhileDetected>c__Iterator1E.<>f__this = this;
		return <UpdateStartWhileDetected>c__Iterator1E;
	}
}
