using System;
using UnityEngine;

public class mutantFireBombSetup : MonoBehaviour
{
	private bool wet;

	private bool broken;

	public GameObject fireGo;

	public GameObject damageGo;

	public MasterFireSpread fireSpread;

	public GameObject contactFireGo;

	public GameObject gridBlockerGo;

	private void Start()
	{
		base.Invoke("enableHitEffect", 0.5f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Water"))
		{
			this.wet = true;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!this.wet && !this.broken)
		{
			this.doBreak();
		}
		base.Invoke("StartCleanUp", 1f);
	}

	private void doBreak()
	{
		this.broken = true;
		this.contactFireGo.SetActive(true);
		this.contactFireGo.transform.parent = null;
		base.GetComponent<Rigidbody>().isKinematic = true;
		UnityEngine.Object.Instantiate(this.gridBlockerGo, base.transform.position, base.transform.rotation);
	}

	private void StartCleanUp()
	{
		if (!BoltNetwork.isClient)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void enableFire()
	{
		if (this.fireGo)
		{
			this.fireGo.SetActive(true);
		}
	}

	private void enableHitEffect()
	{
		if (this.damageGo)
		{
			this.damageGo.SetActive(true);
		}
		if (this.fireSpread)
		{
			this.fireSpread.enabled = true;
		}
	}

	private void OnDisable()
	{
		if (this.fireGo)
		{
			this.fireGo.SetActive(false);
		}
		if (this.damageGo)
		{
			this.damageGo.SetActive(false);
		}
	}
}
