using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween"), HutongGames.PlayMaker.Tooltip("Stop an iTween action.")]
	public class iTweenStop : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmString id;

		public iTweenFSMType iTweenType;

		public bool includeChildren;

		public bool inScene;

		public override void Reset()
		{
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.iTweenType = iTweenFSMType.all;
			this.includeChildren = false;
			this.inScene = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.DoiTween();
			base.Finish();
		}

		private void DoiTween()
		{
			if (this.id.IsNone)
			{
				if (this.iTweenType == iTweenFSMType.all)
				{
					iTween.Stop();
				}
				else if (this.inScene)
				{
					iTween.Stop(Enum.GetName(typeof(iTweenFSMType), this.iTweenType));
				}
				else
				{
					GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
					if (ownerDefaultTarget == null)
					{
						return;
					}
					iTween.Stop(ownerDefaultTarget, Enum.GetName(typeof(iTweenFSMType), this.iTweenType), this.includeChildren);
				}
			}
			else
			{
				iTween.StopByName(this.id.Value);
			}
		}
	}
}
