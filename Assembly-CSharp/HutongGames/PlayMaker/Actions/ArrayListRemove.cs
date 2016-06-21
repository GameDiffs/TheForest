using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), Tooltip("Remove an element of a PlayMaker Array List Proxy component")]
	public class ArrayListRemove : ArrayListActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)"), UIHint(UIHint.FsmString)]
		public FsmString reference;

		[ActionSection("Data"), Tooltip("The type of Variable to remove.")]
		public FsmVar variable;

		[ActionSection("Result"), Tooltip("Event sent if this arraList does not contains that element ( described below)"), UIHint(UIHint.FsmEvent)]
		public FsmEvent notFoundEvent;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.notFoundEvent = null;
			this.variable = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoRemoveFromArrayList();
			}
			base.Finish();
		}

		public void DoRemoveFromArrayList()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			if (!this.proxy.Remove(PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variable), this.variable.Type.ToString(), false))
			{
				base.Fsm.Event(this.notFoundEvent);
			}
		}
	}
}
