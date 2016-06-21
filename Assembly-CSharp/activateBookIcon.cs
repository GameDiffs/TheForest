using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

public class activateBookIcon : MonoBehaviour
{
	private bool inTrigger;

	public float range = 20f;

	public GameObject MyPickUp;

	private PlayerInventory Player;

	private void Start()
	{
		this.inTrigger = false;
		this.MyPickUp.SetActive(false);
	}

	private void Update()
	{
		if (LocalPlayer.MainCamTr)
		{
			Vector3 from = this.MyPickUp.transform.position - LocalPlayer.MainCamTr.position;
			float num = Vector3.Angle(from, LocalPlayer.MainCam.transform.forward);
			if (num < this.range)
			{
				this.MyPickUp.SetActive(true);
			}
			else
			{
				this.MyPickUp.SetActive(false);
			}
			if (num < this.range && TheForest.Utils.Input.GetButtonDown("Take"))
			{
				this.MyPickUp.SetActive(false);
				Scene.TriggerCutScene.getBook = true;
				base.transform.parent.gameObject.SetActive(false);
			}
		}
	}
}
