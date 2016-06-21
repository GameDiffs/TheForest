using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween"), HutongGames.PlayMaker.Tooltip("Resume an iTween action.")]
	public class iTweenResume : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public iTweenFSMType iTweenType;

		public bool includeChildren;

		public bool inScene;

		public override void Reset()
		{
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
			if (this.iTweenType == iTweenFSMType.all)
			{
				iTween.Resume();
			}
			else if (this.inScene)
			{
				iTween.Resume(Enum.GetName(typeof(iTweenFSMType), this.iTweenType));
			}
			else
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				iTween.Resume(ownerDefaultTarget, Enum.GetName(typeof(iTweenFSMType), this.iTweenType), this.includeChildren);
			}
		}
	}
}
