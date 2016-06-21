using System;
using TheForest.Buildings.Creation;
using TheForest.Items.Craft;

public class CoopConstruction : CoopBase<IConstructionState>
{
	private CachedLocal<Craft_Structure> _cs;

	private void Awake()
	{
		this._cs = new CachedLocal<Craft_Structure>(base.gameObject);
	}

	public override void Attached()
	{
		if (this.entity.isOwner)
		{
			base.enabled = false;
		}
		else if (this._cs.Component)
		{
			this._cs.Component.UpdateNetworkIngredients();
		}
	}

	private void FixedUpdate()
	{
		if (this.entity && this.entity.isAttached && this._cs.Component)
		{
			bool flag = false;
			for (int i = 0; i < this._cs.Component.GetPresentIngredients().Length; i++)
			{
				ReceipeIngredient receipeIngredient = this._cs.Component.GetPresentIngredients()[i];
				if (receipeIngredient != null && receipeIngredient._amount != base.state.Ingredients[i].Count)
				{
					receipeIngredient._amount = base.state.Ingredients[i].Count;
					flag = true;
				}
			}
			if (flag)
			{
				this._cs.Component.UpdateNeededRenderers();
			}
		}
	}
}
