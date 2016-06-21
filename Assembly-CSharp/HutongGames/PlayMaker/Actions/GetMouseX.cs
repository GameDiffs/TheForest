using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Gets the X Position of the mouse and stores it in a Float Variable.")]
	public class GetMouseX : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmFloat storeResult;

		public bool normalize;

		public override void Reset()
		{
			this.storeResult = null;
			this.normalize = true;
		}

		public override void OnEnter()
		{
			this.DoGetMouseX();
		}

		public override void OnUpdate()
		{
			this.DoGetMouseX();
		}

		private void DoGetMouseX()
		{
			if (this.storeResult != null)
			{
				float num = Input.mousePosition.x;
				if (this.normalize)
				{
					num /= (float)Screen.width;
				}
				this.storeResult.Value = num;
			}
		}
	}
}
