using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class PlayerWeapons : MonoBehaviour
{
	[CompilerGenerated]
	[Serializable]
	internal sealed class $AxeAttack$736 : GenericGenerator<WaitForSeconds>
	{
		internal PlayerWeapons $self_$738;

		public $AxeAttack$736(PlayerWeapons self_)
		{
			this.$self_$738 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new PlayerWeapons.$AxeAttack$736.$(this.$self_$738);
		}
	}

	public GameObject AxeHeld;

	public GameObject Axe;

	public GameObject AxeCut;

	public Transform AxeCutPos;

	private bool Attacking;

	public override void Update()
	{
		if (Input.GetButtonDown("GiveAxe"))
		{
			this.AxeHeld.SetActive(true);
		}
		if (this.AxeHeld.activeSelf)
		{
			if (Input.GetButtonDown("Drop"))
			{
				this.DropAxe();
			}
			if (Input.GetButtonDown("Fire1"))
			{
				this.StartCoroutine_Auto(this.AxeAttack());
			}
		}
	}

	public override IEnumerator AxeAttack()
	{
		return new PlayerWeapons.$AxeAttack$736(this).GetEnumerator();
	}

	public override void GotAxe()
	{
		this.AxeHeld.SetActive(true);
	}

	public override void DropAxe()
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.Axe, this.AxeHeld.transform.position, this.AxeHeld.transform.rotation);
		this.AxeHeld.SetActive(false);
	}

	public override void Main()
	{
	}
}
