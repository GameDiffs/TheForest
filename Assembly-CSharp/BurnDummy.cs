using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items;
using UnityEngine;

public class BurnDummy : EntityBehaviour
{
	public GameObject _infectionTrigger;

	public Transform[] _bonesSpawn;

	public Transform _skullSpawn;

	public GameObject _pickupTrigger;

	public GameObject _fire;

	public Renderer[] _renderers;

	public Material _burntMat;

	[ItemIdPicker]
	public int _boneItemId;

	[ItemIdPicker]
	public int _skullItemId;

	public bool _isBurning;

	private void Burn()
	{
		if (!BoltNetwork.isClient && !this._isBurning)
		{
			base.StartCoroutine(this.BurnRoutine());
			if (BoltNetwork.isServer && this.entity.isAttached)
			{
				if (this.entity.StateIs<IMutantMaleDummyState>())
				{
					this.entity.GetState<IMutantMaleDummyState>().IsBurning = true;
				}
				else if (this.entity.StateIs<IMutantFemaleDummyState>())
				{
					this.entity.GetState<IMutantFemaleDummyState>().IsBurning = true;
				}
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator BurnRoutine()
	{
		BurnDummy.<BurnRoutine>c__Iterator2E <BurnRoutine>c__Iterator2E = new BurnDummy.<BurnRoutine>c__Iterator2E();
		<BurnRoutine>c__Iterator2E.<>f__this = this;
		return <BurnRoutine>c__Iterator2E;
	}

	private void BurnSFX()
	{
		if (this._infectionTrigger)
		{
			UnityEngine.Object.Destroy(this._infectionTrigger);
			this._infectionTrigger = null;
		}
		for (int i = 0; i < this._renderers.Length; i++)
		{
			if (this._renderers[i])
			{
				this._renderers[i].sharedMaterial = this._burntMat;
			}
		}
		this._fire.SetActive(true);
		this._pickupTrigger.SetActive(false);
	}

	public override void Attached()
	{
		if (this.entity.StateIs<IMutantMaleDummyState>())
		{
			this.entity.GetState<IMutantMaleDummyState>().AddCallback("IsBurning", new PropertyCallbackSimple(this.BurnSFX));
		}
		else if (this.entity.StateIs<IMutantFemaleDummyState>())
		{
			this.entity.GetState<IMutantFemaleDummyState>().AddCallback("IsBurning", new PropertyCallbackSimple(this.BurnSFX));
		}
	}
}
