using Bolt;
using System;
using TheForest.Utils;
using UnityEngine;

public class TreeCutChunk : EntityBehaviour
{
	public GameObject MyTree;

	public GameObject Fake1;

	public GameObject Fake2;

	public GameObject Fake3;

	public GameObject Fake4;

	private bool Delay;

	public float damage;

	private void Hit(float amount)
	{
		if (this.Delay)
		{
			return;
		}
		this.Delay = true;
		base.Invoke("ResetDelay", 0.5f);
		Prefabs.Instance.SpawnWoodHitPS(base.transform.position, base.transform.rotation);
		if (BoltNetwork.isRunning)
		{
			try
			{
				DamageTree damageTree = DamageTree.Raise(GlobalTargets.OnlyServer);
				damageTree.TreeEntity = base.GetComponentInParent<TreeHealth>().LodEntity;
				damageTree.DamageIndex = int.Parse(base.transform.parent.gameObject.name);
				damageTree.Damage = Mathf.RoundToInt(amount);
				damageTree.Send();
			}
			catch (Exception var_1_9C)
			{
			}
		}
		else
		{
			this.damage += amount;
			if (this.damage >= 1f && this.damage < 2f)
			{
				this.Fake2.SetActive(true);
				this.Fake1.SetActive(false);
			}
			if (this.damage >= 2f && this.damage < 3f)
			{
				this.Fake3.SetActive(true);
				this.Fake2.SetActive(false);
				this.Fake1.SetActive(false);
			}
			if (this.damage >= 3f && this.damage < 4f)
			{
				this.Fake4.SetActive(true);
				this.Fake3.SetActive(false);
				this.Fake2.SetActive(false);
				this.Fake1.SetActive(false);
			}
			if (this.damage >= 4f)
			{
				this.MyTree.SendMessage("Hit");
				UnityEngine.Object.Destroy(base.transform.parent.gameObject);
			}
		}
	}

	private void HitStick()
	{
		Prefabs.Instance.SpawnWoodHitPS(base.transform.position, base.transform.rotation);
	}

	private void ResetDelay()
	{
		this.Delay = false;
	}
}
