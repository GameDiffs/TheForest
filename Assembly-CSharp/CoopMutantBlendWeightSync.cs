using Bolt;
using System;
using UnityEngine;

public class CoopMutantBlendWeightSync : EntityBehaviour<IMutantState>
{
	public int Layer0Index = -1;

	public int Layer1Index = -1;

	public SkinnedMeshRenderer[] Renderers;

	private void Awake()
	{
		if (!BoltNetwork.isRunning)
		{
			base.enabled = false;
		}
	}

	public override void Attached()
	{
		if (!this.entity.IsOwner())
		{
			base.state.AddCallback("BlendShapeWeight0", new PropertyCallbackSimple(this.BlendShapeWeight0Changed));
			base.state.AddCallback("BlendShapeWeight1", new PropertyCallbackSimple(this.BlendShapeWeight1Changed));
		}
	}

	private void BlendShapeWeight0Changed()
	{
		for (int i = 0; i < this.Renderers.Length; i++)
		{
			this.Renderers[i].SetBlendShapeWeight(this.Layer0Index, base.state.BlendShapeWeight0);
		}
	}

	private void BlendShapeWeight1Changed()
	{
		for (int i = 0; i < this.Renderers.Length; i++)
		{
			this.Renderers[i].SetBlendShapeWeight(this.Layer1Index, base.state.BlendShapeWeight1);
		}
	}

	private void Update()
	{
		if (this.entity.IsOwner() && this.Renderers.Length > 0)
		{
			if (this.Layer0Index != -1)
			{
				float blendShapeWeight = this.Renderers[0].GetBlendShapeWeight(this.Layer0Index);
				if (blendShapeWeight != base.state.BlendShapeWeight0)
				{
					base.state.BlendShapeWeight0 = blendShapeWeight;
				}
			}
			if (this.Layer1Index != -1)
			{
				float blendShapeWeight2 = this.Renderers[0].GetBlendShapeWeight(this.Layer1Index);
				if (blendShapeWeight2 != base.state.BlendShapeWeight1)
				{
					base.state.BlendShapeWeight1 = blendShapeWeight2;
				}
			}
		}
	}
}
