using Bolt;
using System;
using TheForest.Utils;
using UnityEngine;

public class DamageCorpse : EntityBehaviour
{
	public int Health;

	public GameObject BloodSplat;

	public GameObject MyCut;

	public GameObject MyGore;

	public GameObject MyPart;

	public GameObject MyGoreSkinny;

	public GameObject MyPartSkinny;

	public GameObject[] props;

	public GameObject femaleClothesCombined;

	public GameObject femaleSkirtOnly;

	public bool ignoreHit;

	public bool infected;

	public void DoLocalCut(int health)
	{
		if (health >= 20)
		{
			return;
		}
		UnityEngine.Object.Destroy(UnityEngine.Object.Instantiate(this.BloodSplat, base.transform.position, Quaternion.identity), 0.5f);
		this.MyGore.SetActive(true);
		if (this.MyGoreSkinny)
		{
			this.MyGoreSkinny.SetActive(true);
		}
		this.Health = health;
		if (health <= 0)
		{
			this.CutDown();
		}
	}

	private void ignoreCutting()
	{
		this.ignoreHit = true;
	}

	private void Hit(int damage)
	{
		if (this.ignoreHit)
		{
			this.ignoreHit = false;
			return;
		}
		if (this.infected)
		{
			LocalPlayer.Stats.BloodInfection.TryGetInfected();
		}
		if (this.entity.IsAttached())
		{
			HitCorpse hitCorpse = HitCorpse.Create(GlobalTargets.OnlyServer);
			hitCorpse.Entity = this.entity;
			hitCorpse.Damage = damage;
			hitCorpse.BodyPartIndex = base.GetComponentInParent<CoopSliceAndDiceMutant>().GetBodyPartIndex(this);
			hitCorpse.Send();
		}
		else
		{
			this.DoLocalCut(this.Health - damage);
		}
	}

	private void CutDown()
	{
		this.MyCut.SetActive(true);
		this.MyPart.SetActive(false);
		if (this.femaleClothesCombined && this.femaleClothesCombined.activeSelf)
		{
			this.femaleClothesCombined.SetActive(false);
			if (this.femaleSkirtOnly)
			{
				this.femaleSkirtOnly.SetActive(true);
			}
		}
		if (this.MyPartSkinny)
		{
			this.MyPartSkinny.SetActive(false);
		}
		if (this.props.Length > 0)
		{
			for (int i = 0; i < this.props.Length; i++)
			{
				this.props[i].SetActive(false);
			}
		}
		if (LocalPlayer.Transform && Vector3.Distance(LocalPlayer.Transform.position, base.transform.position) < 4f)
		{
			LocalPlayer.Stats.Sanity.OnCutLimbOff();
		}
		this.MyCut.transform.parent = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Explosion(float dist)
	{
		base.Invoke("CutDown", 1f);
	}
}
