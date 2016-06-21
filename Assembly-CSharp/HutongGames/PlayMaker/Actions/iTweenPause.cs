using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween"), HutongGames.PlayMaker.Tooltip("Pause an iTween action.")]
	public class iTweenPause : FsmStateAction
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
				iTween.Pause();
			}
			else if (this.inScene)
			{
				iTween.Pause(Enum.GetName(typeof(iTweenFSMType), this.iTweenType));
			}
			else
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				iTween.Pause(ownerDefaultTarget, Enum.GetName(typeof(iTweenFSMType), this.iTweenType), this.includeChildren);
			}
		}
	}
}
