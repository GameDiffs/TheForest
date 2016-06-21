using Bolt;
using System;
using TheForest.Networking;
using UniLinq;
using UnityEngine;

public class itemConstrainToHand : EntityBehaviour
{
	public float xOffset;

	public float yOffset;

	public float zOffset;

	public bool fixedItems;

	public bool toLeftHand;

	public bool isPlayerNet;

	private Transform tr;

	private Transform leftHandWeapon;

	private Transform rightHand;

	private Transform feet;

	public GameObject[] Available;

	public Transform ActiveItem
	{
		get;
		set;
	}

	private void Start()
	{
		base.Invoke("SetupWeapons", 0.3f);
	}

	public void SetupWeapons()
	{
		if (!this.fixedItems)
		{
			if (!this.isPlayerNet || !Application.isPlaying)
			{
				this.Available = new GameObject[base.transform.childCount];
				for (int i = 0; i < this.Available.Length; i++)
				{
					this.Available[i] = base.transform.GetChild(i).gameObject;
					this.Available[i].SetActive(this.isPlayerNet);
				}
			}
			this.Available = (from x in this.Available
			orderby x.name
			select x).ToArray<GameObject>();
		}
	}

	public void Enable(int index, StealItemTrigger RightHandStealTrigger = null)
	{
		for (int i = 0; i < this.Available.Length; i++)
		{
			if (index != i && this.Available[i].activeSelf)
			{
				this.Available[i].SetActive(false);
			}
		}
		if (index != -1 && index < this.Available.Length)
		{
			if (!this.Available[index].activeSelf)
			{
				this.Available[index].SetActive(true);
			}
			this.ActiveItem = this.Available[index].transform;
			if (RightHandStealTrigger)
			{
				RightHandStealTrigger.ActivateIfIsStealableItem(this.Available[index]);
			}
		}
		else if (RightHandStealTrigger)
		{
			RightHandStealTrigger.gameObject.SetActive(false);
		}
	}

	public void Parent()
	{
		this.tr = base.transform;
		Transform[] componentsInChildren = base.transform.root.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i];
			if (transform.name == "char_RightHandWeapon")
			{
				this.rightHand = transform;
			}
			if (transform.name == "char_LeftHandWeapon")
			{
				this.leftHandWeapon = transform;
			}
		}
	}

	private void parentToRightHand()
	{
		base.transform.parent = this.rightHand;
		this.tr.position = this.rightHand.position;
		this.tr.rotation = this.rightHand.rotation;
	}

	private void parentToLeftHand()
	{
		base.transform.parent = this.leftHandWeapon;
		this.tr.position = this.leftHandWeapon.position;
		this.tr.rotation = this.leftHandWeapon.rotation;
	}
}
