using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), HutongGames.PlayMaker.Tooltip("Remove item at a specified index of a PlayMaker ArrayList Proxy component")]
	public class ArrayListRemoveAt : ArrayListActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, HutongGames.PlayMaker.Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[HutongGames.PlayMaker.Tooltip("The index to remove at"), UIHint(UIHint.FsmInt)]
		public FsmInt index;

		[ActionSection("Result"), HutongGames.PlayMaker.Tooltip("The event to trigger if the removeAt throw errors"), UIHint(UIHint.FsmEvent)]
		public FsmEvent failureEvent;

		public override void Reset()
		{
			this.gameObject = null;
			this.failureEvent = null;
			this.reference = null;
			this.index = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.doArrayListRemoveAt();
			}
			base.Finish();
		}

		public void doArrayListRemoveAt()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			try
			{
				this.proxy.arrayList.RemoveAt(this.index.Value);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				base.Fsm.Event(this.failureEvent);
			}
		}
	}
}
