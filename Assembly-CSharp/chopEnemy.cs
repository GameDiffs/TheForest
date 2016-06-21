using Bolt;
using System;
using UnityEngine;

public class chopEnemy : MonoBehaviour
{
	public GameObject RagDollExploded;

	public Transform hips;

	public GameObject rootGo;

	public GameObject destroyGo;

	public bool encounterBool;

	public bool sendTrapReset;

	private bool blockTrigger;

	private bool doChop;

	private void Start()
	{
		base.Invoke("enableChopable", 2f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Weapon") && this.doChop)
		{
			if (BoltNetwork.isRunning)
			{
				Chop chop = Chop.Raise(GlobalTargets.Everyone);
				chop.Target = base.GetComponentInParent<BoltEntity>();
				chop.Send();
			}
			else
			{
				this.triggerChop();
			}
		}
	}

	private void enableChopable()
	{
		this.doChop = true;
	}

	public void triggerChop()
	{
		if (this.encounterBool)
		{
			UnityEngine.Object.Instantiate(this.RagDollExploded, this.rootGo.transform.position, this.rootGo.transform.rotation);
		}
		else
		{
			UnityEngine.Object.Instantiate(this.RagDollExploded, this.hips.transform.position, this.hips.transform.rotation);
		}
		if (this.sendTrapReset)
		{
			base.SendMessageUpwards("enableTrapReset", SendMessageOptions.DontRequireReceiver);
		}
		if (BoltNetwork.isRunning)
		{
			for (int i = 0; i < this.hips.parent.childCount; i++)
			{
				this.hips.parent.GetChild(i).gameObject.SetActive(false);
			}
		}
		if (this.encounterBool)
		{
			if (BoltNetwork.isServer)
			{
				base.GetComponentInParent<BoltEntity>().DestroyDelayed(2f);
			}
			else if (!BoltNetwork.isRunning)
			{
				UnityEngine.Object.Destroy(this.destroyGo);
			}
		}
		else if (BoltNetwork.isServer)
		{
			base.GetComponentInParent<BoltEntity>().DestroyDelayed(2f);
		}
		else if (!BoltNetwork.isRunning)
		{
			UnityEngine.Object.Destroy(this.rootGo);
		}
	}
}
