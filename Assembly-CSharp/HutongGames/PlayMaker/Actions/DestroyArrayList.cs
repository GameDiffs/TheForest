using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), HutongGames.PlayMaker.Tooltip("Destroys a PlayMakerArrayListProxy Component of a Game Object.")]
	public class DestroyArrayList : ArrayListActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, HutongGames.PlayMaker.Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker ArrayList proxy component ( necessary if several component coexists on the same GameObject"), UIHint(UIHint.FsmString)]
		public FsmString reference;

		[ActionSection("Result"), HutongGames.PlayMaker.Tooltip("The event to trigger if the ArrayList proxy component is destroyed"), UIHint(UIHint.FsmEvent)]
		public FsmEvent successEvent;

		[HutongGames.PlayMaker.Tooltip("The event to trigger if the ArrayList proxy component was not found"), UIHint(UIHint.FsmEvent)]
		public FsmEvent notFoundEvent;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.successEvent = null;
			this.notFoundEvent = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoDestroyArrayList();
			}
			else
			{
				base.Fsm.Event(this.notFoundEvent);
			}
			base.Finish();
		}

		private void DoDestroyArrayList()
		{
			PlayMakerArrayListProxy[] components = this.proxy.GetComponents<PlayMakerArrayListProxy>();
			PlayMakerArrayListProxy[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				PlayMakerArrayListProxy playMakerArrayListProxy = array[i];
				if (playMakerArrayListProxy.referenceName == this.reference.Value)
				{
					UnityEngine.Object.Destroy(playMakerArrayListProxy);
					base.Fsm.Event(this.successEvent);
					return;
				}
			}
			base.Fsm.Event(this.notFoundEvent);
		}
	}
}
