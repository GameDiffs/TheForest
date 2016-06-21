using Bolt;
using System;

public abstract class CoopSliceAndDiceMutant : EntityBehaviour<IMutantState>
{
	public DamageCorpse[] BodyParts;

	public abstract NetworkArray_Integer BodyPartsDamage
	{
		get;
	}

	public override void Attached()
	{
		base.state.AddCallback("BodyPartsDamage[]", new PropertyCallback(this.BodyPartDamaged));
		this.BodyPartsDamage[0] = 20;
		this.BodyPartsDamage[1] = 20;
		this.BodyPartsDamage[2] = 20;
		this.BodyPartsDamage[3] = 20;
		this.BodyPartsDamage[4] = 20;
	}

	private void BodyPartDamaged(IState state, string path, ArrayIndices indices)
	{
		this.BodyParts[indices[0]].DoLocalCut(this.BodyPartsDamage[indices[0]]);
	}

	public int GetBodyPartIndex(DamageCorpse corpse)
	{
		return Array.IndexOf<DamageCorpse>(this.BodyParts, corpse);
	}
}
