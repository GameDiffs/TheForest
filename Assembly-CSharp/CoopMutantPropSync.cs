using Bolt;
using System;
using UnityEngine;

public class CoopMutantPropSync : EntityBehaviour<IMutantState>
{
	public bool DisableByDefaultOnClient;

	public GameObject[] Props;

	private void Awake()
	{
		if (!BoltNetwork.isRunning)
		{
			base.enabled = false;
		}
		else if (BoltNetwork.isClient && this.DisableByDefaultOnClient)
		{
			for (int i = 0; i < this.Props.Length; i++)
			{
				if (this.Props[i])
				{
					this.Props[i].SetActive(false);
				}
			}
		}
	}

	public void ApplyPropMask(int props)
	{
		for (int i = 0; i < this.Props.Length; i++)
		{
			if (this.Props[i])
			{
				int num = 1 << i;
				this.Props[i].SetActive((props & num) == num);
			}
		}
	}

	public override void Attached()
	{
		if (!this.entity.IsOwner())
		{
			base.state.AddCallback("prop_mask", delegate
			{
				this.ApplyPropMask(base.state.prop_mask);
			});
		}
	}

	private void Update()
	{
		if (this.entity.IsOwner())
		{
			int num = 0;
			for (int i = 0; i < this.Props.Length; i++)
			{
				if (this.Props[i] && this.Props[i].activeInHierarchy)
				{
					num |= 1 << i;
				}
				if (base.state.prop_mask != num)
				{
					base.state.prop_mask = num;
				}
			}
		}
	}
}
