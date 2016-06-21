using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Substance"), HutongGames.PlayMaker.Tooltip("Set a named float property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
	public class SetProceduralFloat : FsmStateAction
	{
		[RequiredField]
		public FsmMaterial substanceMaterial;

		[RequiredField]
		public FsmString floatProperty;

		[RequiredField]
		public FsmFloat floatValue;

		[HutongGames.PlayMaker.Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
		public bool everyFrame;

		public override void Reset()
		{
			this.substanceMaterial = null;
			this.floatProperty = string.Empty;
			this.floatValue = 0f;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetProceduralFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetProceduralFloat();
		}

		private void DoSetProceduralFloat()
		{
			ProceduralMaterial proceduralMaterial = this.substanceMaterial.Value as ProceduralMaterial;
			if (proceduralMaterial == null)
			{
				this.LogError("Not a substance material!");
				return;
			}
			proceduralMaterial.SetProceduralFloat(this.floatProperty.Value, this.floatValue.Value);
		}
	}
}
