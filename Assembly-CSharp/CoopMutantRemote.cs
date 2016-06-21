using Bolt;
using System;

public class CoopMutantRemote : EntityBehaviour<IMutantState>
{
	private setupBodyVariation bodyVariation;

	public override void Attached()
	{
		this.bodyVariation = base.GetComponentInChildren<setupBodyVariation>();
		if (base.state is IMutantFemaleState)
		{
		}
	}
}
