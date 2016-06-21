using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Substance"), HutongGames.PlayMaker.Tooltip("Set a named bool property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
	public class SetProceduralBoolean : FsmStateAction
	{
		[RequiredField]
		public FsmMaterial substanceMaterial;

		[RequiredField]
		public FsmString boolProperty;

		[RequiredField]
		public FsmBool boolValue;

		[HutongGames.PlayMaker.Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
		public bool everyFrame;

		public override void Reset()
		{
			this.substanceMaterial = null;
			this.boolProperty = string.Empty;
			this.boolValue = false;
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
			proceduralMaterial.SetProceduralBoolean(this.boolProperty.Value, this.boolValue.Value);
		}
	}
}
