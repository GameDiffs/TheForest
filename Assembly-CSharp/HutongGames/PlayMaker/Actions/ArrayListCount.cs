using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), Tooltip("Count items from a PlayMaker ArrayList Proxy component")]
	public class ArrayListCount : ArrayListActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[ActionSection("Result"), Tooltip("Store the count value"), UIHint(UIHint.FsmInt)]
		public FsmInt count;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.count = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.getArrayListCount();
			}
			base.Finish();
		}

		public void getArrayListCount()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			int value = this.proxy.arrayList.Count;
			this.count.Value = value;
		}
	}
}
