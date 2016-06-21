using System;
using TheForest.Utils;
using UnityEngine;

public class Mud : MonoBehaviour
{
	public GameObject Sheen;

	public GameObject PickUp;

	private GameObject Player;

	private void Awake()
	{
		this.Player = GameObject.FindWithTag("Player");
		base.enabled = false;
	}

	private void GrabEnter()
	{
		base.enabled = true;
		this.Sheen.SetActive(false);
		this.PickUp.SetActive(true);
	}

	private void GrabExit()
	{
		base.enabled = false;
		this.Sheen.SetActive(true);
		this.PickUp.SetActive(false);
	}

	private void Update()
	{
		if (TheForest.Utils.Input.GetButtonDown("Take"))
		{
			this.Player.SendMessage("GotMud");
			base.enabled = false;
			UnityEngine.Object.Destroy(base.transform.parent.gameObject);
		}
	}
}
