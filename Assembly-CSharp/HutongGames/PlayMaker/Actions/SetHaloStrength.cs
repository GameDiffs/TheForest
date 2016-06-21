using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.RenderSettings), HutongGames.PlayMaker.Tooltip("Sets the size of light halos.")]
	public class SetHaloStrength : FsmStateAction
	{
		[RequiredField]
		public FsmFloat haloStrength;

		public bool everyFrame;

		public override void Reset()
		{
			this.haloStrength = 0.5f;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetHaloStrength();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetHaloStrength();
		}

		private void DoSetHaloStrength()
		{
			RenderSettings.haloStrength = this.haloStrength.Value;
		}
	}
}
