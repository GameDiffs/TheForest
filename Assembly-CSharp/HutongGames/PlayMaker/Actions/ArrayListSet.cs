using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), Tooltip("Set an item at a specified index to a PlayMaker array List component")]
	public class ArrayListSet : ArrayListActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)"), UIHint(UIHint.FsmString)]
		public FsmString reference;

		[Tooltip("The index of the Data in the ArrayList"), UIHint(UIHint.FsmString)]
		public FsmInt atIndex;

		public bool everyFrame;

		[ActionSection("Data"), Tooltip("The variable to add.")]
		public FsmVar variable;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.variable = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.SetToArrayList();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.SetToArrayList();
		}

		public void SetToArrayList()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.Set(this.atIndex.Value, PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variable), this.variable.Type.ToString());
		}
	}
}
