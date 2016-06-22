using System;
using UnityEngine;

[Serializable]
public class TriggerBats : MonoBehaviour
{
	private bool Playing;

	public GameObject Bats;

	public int initialDelay;

	public int repeatDelay;

	public bool activated;

	public bool alwaysRepeat;

	public override void Start()
	{
		if (this.alwaysRepeat && this.repeatDelay > 0)
		{
			this.Invoke("resetBats", 2.7f);
			this.InvokeRepeating("repeatBats", (float)this.initialDelay, (float)this.repeatDelay);
			this.activated = true;
		}
	}

	public override void OnTriggerEnter(Collider otherObject)
	{
		if (!this.activated)
		{
			if (otherObject.gameObject.CompareTag("Player"))
			{
				this.Bats.SetActive(true);
				this.Invoke("BatsAnim", 0.1f);
				if (this.repeatDelay > 0)
				{
					this.Invoke("resetBats", 2.3f);
					this.InvokeRepeating("repeatBats", (float)this.repeatDelay, (float)this.repeatDelay);
					this.activated = true;
				}
				this.Invoke("KillMe", 2.3f);
			}
		}
	}

	public override void OnTriggerExit(Collider otherObject)
	{
		if (otherObject.gameObject.CompareTag("Player"))
		{
			this.Bats.SetActive(false);
			this.CancelInvoke("repeatBats");
			this.activated = false;
		}
	}

	public override void BatsAnim()
	{
		this.Bats.BroadcastMessage("doBats");
	}

	public override void resetBats()
	{
		this.Bats.SetActive(false);
	}

	public override void repeatBats()
	{
		this.Bats.SetActive(true);
		this.Bats.BroadcastMessage("doBats");
		this.Invoke("resetBats", 2.7f);
	}

	public override void KillMe()
	{
		UnityEngine.Object.Destroy(this.gameObject);
	}

	public override void Main()
	{
	}
}
