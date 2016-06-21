using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), Tooltip("Sorts the sequence of elements in a PlayMaker ArrayList Proxy component")]
	public class ArrayListSort : ArrayListActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoArrayListSort();
			}
			base.Finish();
		}

		public void DoArrayListSort()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.arrayList.Sort();
		}
	}
}
