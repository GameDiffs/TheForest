using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3), HutongGames.PlayMaker.Tooltip("Use a low pass filter to reduce the influence of sudden changes in a Vector3 Variable. Useful when working with Get Device Acceleration to isolate gravity.")]
	public class Vector3LowPassFilter : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("Vector3 Variable to filter. Should generally come from some constantly updated input, e.g., acceleration."), UIHint(UIHint.Variable)]
		public FsmVector3 vector3Variable;

		[HutongGames.PlayMaker.Tooltip("Determines how much influence new changes have. E.g., 0.1 keeps 10 percent of the unfiltered vector and 90 percent of the previously filtered value.")]
		public FsmFloat filteringFactor;

		private Vector3 filteredVector;

		public override void Reset()
		{
			this.vector3Variable = null;
			this.filteringFactor = 0.1f;
		}

		public override void OnEnter()
		{
			this.filteredVector = new Vector3(this.vector3Variable.Value.x, this.vector3Variable.Value.y, this.vector3Variable.Value.z);
		}

		public override void OnUpdate()
		{
			this.filteredVector.x = this.vector3Variable.Value.x * this.filteringFactor.Value + this.filteredVector.x * (1f - this.filteringFactor.Value);
			this.filteredVector.y = this.vector3Variable.Value.y * this.filteringFactor.Value + this.filteredVector.y * (1f - this.filteringFactor.Value);
			this.filteredVector.z = this.vector3Variable.Value.z * this.filteringFactor.Value + this.filteredVector.z * (1f - this.filteringFactor.Value);
			this.vector3Variable.Value = new Vector3(this.filteredVector.x, this.filteredVector.y, this.filteredVector.z);
		}
	}
}
