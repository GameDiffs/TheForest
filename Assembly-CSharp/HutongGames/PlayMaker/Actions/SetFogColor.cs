using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.RenderSettings), HutongGames.PlayMaker.Tooltip("Sets the color of the Fog in the scene.")]
	public class SetFogColor : FsmStateAction
	{
		[RequiredField]
		public FsmColor fogColor;

		public bool everyFrame;

		public override void Reset()
		{
			this.fogColor = Color.white;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetFogColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetFogColor();
		}

		private void DoSetFogColor()
		{
			RenderSettings.fogColor = this.fogColor.Value;
		}
	}
}
