using Bolt;
using System;

public class CoopSliceAndDiceMutantFemale : CoopSliceAndDiceMutant
{
	public override NetworkArray_Integer BodyPartsDamage
	{
		get
		{
			return this.entity.GetState<IMutantFemaleDummyState>().BodyPartsDamage;
		}
	}
}
