using Bolt;
using System;
using UnityEngine;

public class CoopEffigy : EntityBehaviour<IBuildingEffigyState>
{
	public enableEffigy effigy;

	public Transform partsRoot;

	public override void Attached()
	{
		base.state.AddCallback("Lit", new PropertyCallbackSimple(this.OnLitChanged));
	}

	private void OnLitChanged()
	{
		if (base.state.Lit)
		{
			this.effigy.lightEffigyReal();
		}
		else
		{
			this.effigy.dieReal();
		}
	}
}
