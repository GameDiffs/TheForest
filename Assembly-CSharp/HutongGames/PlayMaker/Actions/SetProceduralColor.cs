using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Substance"), HutongGames.PlayMaker.Tooltip("Set a named color property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
	public class SetProceduralColor : FsmStateAction
	{
		[RequiredField]
		public FsmMaterial substanceMaterial;

		[RequiredField]
		public FsmString colorProperty;

		[RequiredField]
		public FsmColor colorValue;

		[HutongGames.PlayMaker.Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
		public bool everyFrame;

		public override void Reset()
		{
			this.substanceMaterial = null;
			this.colorProperty = string.Empty;
			this.colorValue = Color.white;
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
			proceduralMaterial.SetProceduralColor(this.colorProperty.Value, this.colorValue.Value);
		}
	}
}
