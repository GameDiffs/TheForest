using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.RenderSettings), HutongGames.PlayMaker.Tooltip("Sets the intensity of all Flares in the scene.")]
	public class SetFlareStrength : FsmStateAction
	{
		[RequiredField]
		public FsmFloat flareStrength;

		public bool everyFrame;

		public override void Reset()
		{
			this.flareStrength = 0.2f;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetFlareStrength();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetFlareStrength();
		}

		private void DoSetFlareStrength()
		{
			RenderSettings.flareStrength = this.flareStrength.Value;
		}
	}
}
