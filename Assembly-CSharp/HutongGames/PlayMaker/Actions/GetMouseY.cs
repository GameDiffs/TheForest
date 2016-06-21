using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Gets the Y Position of the mouse and stores it in a Float Variable.")]
	public class GetMouseY : FsmStateAction
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
			this.DoGetMouseY();
		}

		public override void OnUpdate()
		{
			this.DoGetMouseY();
		}

		private void DoGetMouseY()
		{
			if (this.storeResult != null)
			{
				float num = Input.mousePosition.y;
				if (this.normalize)
				{
					num /= (float)Screen.height;
				}
				this.storeResult.Value = num;
			}
		}
	}
}
