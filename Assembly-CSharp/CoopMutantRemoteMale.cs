using Bolt;
using System;
using UnityEngine;

public class CoopMutantRemoteMale : EntityBehaviour<IMutantState>
{
	[SerializeField]
	public SkinnedMeshRenderer MeshRenderer;

	public override void Attached()
	{
		if (!this.entity.isOwner)
		{
			base.state.AddCallback("BlendShapeWeight0", delegate
			{
				this.MeshRenderer.SetBlendShapeWeight(28, base.state.BlendShapeWeight0);
			});
		}
	}
}
