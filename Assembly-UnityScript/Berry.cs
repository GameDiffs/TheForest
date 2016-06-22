using System;
using UnityEngine;

[Serializable]
public class Berry : MonoBehaviour
{
	public bool BlueBerry;

	public bool TwinBerry;

	private bool BerryTaken;

	private GameObject Player;

	private bool PlayerHere;

	public GameObject Sheen;

	public GameObject PickUp;

	public override void Awake()
	{
		this.Player = GameObject.FindWithTag("Player");
	}

	public override void OnEnable()
	{
		this.BerryTaken = LOD_Stats.GetBool(this, false);
		this.Sheen.SetActive(!this.BerryTaken);
		this.PickUp.SetActive(false);
		this.GetComponent<Renderer>().enabled = !this.BerryTaken;
		this.GetComponent<Collider>().enabled = !this.BerryTaken;
	}

	public override void OnDisable()
	{
		LOD_Stats.SetBool(this, this.BerryTaken);
	}

	public override void Update()
	{
		if (this.PlayerHere && !this.BerryTaken && Input.GetButtonDown("Take"))
		{
			if (this.BlueBerry)
			{
				this.Player.SendMessage("AteBlueBerry");
			}
			if (this.TwinBerry)
			{
				this.Player.SendMessage("AteTwinBerry");
			}
			this.BerryTaken = true;
			this.Sheen.SetActive(false);
			this.PickUp.SetActive(false);
			this.GetComponent<Renderer>().enabled = false;
			this.GetComponent<Collider>().enabled = false;
		}
	}

	public override void GrabEnter()
	{
		this.PlayerHere = true;
		if (!this.BerryTaken)
		{
			this.Sheen.SetActive(false);
			this.PickUp.SetActive(true);
		}
	}

	public override void GrabExit()
	{
		this.PlayerHere = false;
		if (!this.BerryTaken)
		{
			this.Sheen.SetActive(true);
			this.PickUp.SetActive(false);
		}
	}

	public override void Main()
	{
	}
}
