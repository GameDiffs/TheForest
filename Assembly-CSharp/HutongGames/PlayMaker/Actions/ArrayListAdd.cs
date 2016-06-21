using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), Tooltip("Add an item to a PlayMaker Array List Proxy component")]
	public class ArrayListAdd : ArrayListActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)"), UIHint(UIHint.FsmString)]
		public FsmString reference;

		[ActionSection("Data"), RequiredField, Tooltip("The variable to add.")]
		public FsmVar variable;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.variable = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.AddToArrayList();
			}
			base.Finish();
		}

		public void AddToArrayList()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.Add(PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variable), this.variable.Type.ToString(), false);
		}
	}
}
