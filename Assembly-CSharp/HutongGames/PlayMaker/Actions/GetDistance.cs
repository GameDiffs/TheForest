using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Measures the Distance betweens 2 Game Objects and stores the result in a Float Variable.")]
	public class GetDistance : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmGameObject target;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmFloat storeResult;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.storeResult = null;
			this.everyFrame = true;
		}

		public override void OnEnter()
		{
			this.DoGetDistance();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetDistance();
		}

		private void DoGetDistance()
		{
			GameObject gameObject = (this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? this.gameObject.GameObject.Value : base.Owner;
			if (gameObject == null || this.target.Value == null || this.storeResult == null)
			{
				return;
			}
			this.storeResult.Value = Vector3.Distance(gameObject.transform.position, this.target.Value.transform.position);
		}
	}
}
