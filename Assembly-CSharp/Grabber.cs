using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Grabber : MonoBehaviour
{
	private static bool wasFocused;

	public static GameObject Filter;

	private int PickupLayer = -1;

	private bool busyLock;

	private Collider collider;

	private bool physicsUpdate;

	private bool triggerStayUpdate;

	public static Collider FocusedItem
	{
		get;
		private set;
	}

	public static GameObject FocusedItemGO
	{
		get;
		private set;
	}

	public static bool IsFocused
	{
		get
		{
			return Grabber.FocusedItem && Grabber.FocusedItem.enabled && Grabber.FocusedItemGO.activeInHierarchy;
		}
	}

	private void Awake()
	{
		this.collider = base.GetComponent<Collider>();
		this.PickupLayer = LayerMask.NameToLayer("PickUp");
	}

	private void EnterMessage()
	{
		Grabber.FocusedItem.SendMessage("GrabEnter", base.gameObject, SendMessageOptions.DontRequireReceiver);
	}

	private void ExitMessage()
	{
		if (Grabber.FocusedItem)
		{
			Grabber.FocusedItem.SendMessage("GrabExit", base.gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void RefreshCollider()
	{
		this.collider.enabled = false;
		this.collider.enabled = true;
	}

	private void FixedUpdate()
	{
		this.physicsUpdate = true;
	}

	private void Update()
	{
		if (Grabber.wasFocused && !Grabber.IsFocused)
		{
			this.ExitMessage();
			Grabber.FocusedItem = null;
			Grabber.FocusedItemGO = null;
			Grabber.wasFocused = false;
			this.RefreshCollider();
		}
		else if (Grabber.wasFocused && ((LocalPlayer.Inventory.CurrentView != PlayerInventory.PlayerViews.World && LocalPlayer.Inventory.CurrentView != PlayerInventory.PlayerViews.Sleep) || LocalPlayer.Create.CreateMode))
		{
			this.ExitMessage();
			Grabber.FocusedItem = null;
			Grabber.FocusedItemGO = null;
			Grabber.wasFocused = false;
			this.RefreshCollider();
			this.busyLock = true;
		}
		else if (this.busyLock)
		{
			this.RefreshCollider();
			this.busyLock = false;
		}
		if (this.physicsUpdate)
		{
			if (!this.triggerStayUpdate && Grabber.IsFocused)
			{
				this.ExitMessage();
				Grabber.FocusedItem = null;
				Grabber.FocusedItemGO = null;
				Grabber.wasFocused = false;
				this.RefreshCollider();
			}
			this.physicsUpdate = false;
			this.triggerStayUpdate = false;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.isTrigger && other.gameObject.layer == this.PickupLayer && Grabber.FocusedItem != other && (Grabber.Filter == null || other.gameObject.Equals(Grabber.Filter)))
		{
			if (!Grabber.IsFocused || Vector3.Distance(base.transform.position, other.transform.position) < Vector3.Distance(base.transform.position, Grabber.FocusedItem.transform.position))
			{
				if (Grabber.IsFocused)
				{
					this.ExitMessage();
				}
				Grabber.FocusedItem = other;
				Grabber.FocusedItemGO = other.gameObject;
				this.EnterMessage();
				Grabber.wasFocused = true;
				this.triggerStayUpdate = true;
			}
		}
		else if (Grabber.FocusedItem == other)
		{
			this.triggerStayUpdate = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == this.PickupLayer && other == Grabber.FocusedItem)
		{
			this.ExitMessage();
			Grabber.FocusedItem = null;
			Grabber.FocusedItemGO = null;
			Grabber.wasFocused = false;
			this.RefreshCollider();
		}
	}
}
