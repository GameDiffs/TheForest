using Bolt;
using System;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
	public GameObject Player;

	public GameObject Audio;

	public bool Stick;

	public bool FlameThrower;

	public bool FireStick;

	public bool Axe;

	private bool Rock;

	public float DelayAmount = 1f;

	private bool CanHit = true;

	private bool Swung;

	private void Awake()
	{
		this.Swung = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("SmallTree") && this.CanHit && this.FireStick)
		{
			other.SendMessage("Burn", SendMessageOptions.DontRequireReceiver);
		}
		if ((other.gameObject.CompareTag("enemyCollide") || other.gameObject.CompareTag("animalCollide")) && this.CanHit)
		{
			this.CanHit = false;
			base.Invoke("ResetCanHit", this.DelayAmount);
			if (this.Stick)
			{
				other.SendMessage("Hit", SendMessageOptions.DontRequireReceiver);
				if (base.GetComponent<AudioSource>())
				{
					base.GetComponent<AudioSource>().Play();
				}
			}
			if (this.Rock)
			{
				other.SendMessage("Hit", SendMessageOptions.DontRequireReceiver);
				if (base.GetComponent<AudioSource>())
				{
					base.GetComponent<AudioSource>().Play();
				}
			}
			if (this.FireStick)
			{
				if (BoltNetwork.isRunning)
				{
					BoltEntity componentInParent = other.GetComponentInParent<BoltEntity>();
					if (componentInParent && !componentInParent.isOwner)
					{
						SendMessageEvent sendMessageEvent = SendMessageEvent.Create(componentInParent, EntityTargets.OnlyOwner);
						SendMessageEvent sendMessageEvent2 = SendMessageEvent.Create(componentInParent, EntityTargets.OnlyOwner);
						sendMessageEvent2.Message = "Douse";
						sendMessageEvent2.Target = componentInParent;
						sendMessageEvent2.Send();
						sendMessageEvent.Message = "Burn";
						sendMessageEvent.Target = componentInParent;
						sendMessageEvent.Send();
					}
					else
					{
						other.SendMessage("Douse", SendMessageOptions.DontRequireReceiver);
						other.transform.SendMessage("Burn", SendMessageOptions.DontRequireReceiver);
					}
				}
				else
				{
					other.SendMessage("Douse", SendMessageOptions.DontRequireReceiver);
					other.transform.SendMessage("Burn", SendMessageOptions.DontRequireReceiver);
				}
				if (base.GetComponent<AudioSource>())
				{
					base.GetComponent<AudioSource>().Play();
				}
			}
			if (this.FlameThrower)
			{
				other.SendMessage("Douse", SendMessageOptions.DontRequireReceiver);
				other.SendMessage("Burn", SendMessageOptions.DontRequireReceiver);
			}
			if (this.Axe)
			{
				other.SendMessage("HitAxe", SendMessageOptions.DontRequireReceiver);
				if (base.GetComponent<AudioSource>())
				{
					base.GetComponent<AudioSource>().Play();
				}
			}
		}
		if (other.gameObject.CompareTag("Tree") && this.CanHit && this.Swung && this.Axe)
		{
			this.CanHit = false;
			base.Invoke("ResetCanHit", this.DelayAmount);
			if (base.GetComponent<AudioSource>())
			{
				this.Audio.SendMessage("PlayAxeHit");
			}
			other.SendMessage("Hit");
		}
	}

	private void SwingAxe()
	{
		this.Swung = true;
		base.Invoke("ResetSwung", 2f);
	}

	private void ResetSwung()
	{
		this.Swung = false;
	}

	private void ResetCanHit()
	{
		this.CanHit = true;
	}
}
