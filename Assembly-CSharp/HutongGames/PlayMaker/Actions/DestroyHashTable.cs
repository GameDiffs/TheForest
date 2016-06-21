using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable"), HutongGames.PlayMaker.Tooltip("Destroys a PlayMakerHashTableProxy Component of a Game Object.")]
	public class DestroyHashTable : HashTableActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerHashTableProxy)), RequiredField, HutongGames.PlayMaker.Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker HashTable proxy component ( necessary if several component coexists on the same GameObject"), UIHint(UIHint.FsmString)]
		public FsmString reference;

		[ActionSection("Result"), HutongGames.PlayMaker.Tooltip("The event to trigger if the HashTable proxy component is destroyed"), UIHint(UIHint.FsmEvent)]
		public FsmEvent successEvent;

		[HutongGames.PlayMaker.Tooltip("The event to trigger if the HashTable proxy component was not found"), UIHint(UIHint.FsmEvent)]
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
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.SetUpHashTableProxyPointer(ownerDefaultTarget, this.reference.Value))
			{
				this.DoDestroyHashTable(ownerDefaultTarget);
			}
			else
			{
				base.Fsm.Event(this.notFoundEvent);
			}
			base.Finish();
		}

		private void DoDestroyHashTable(GameObject go)
		{
			PlayMakerHashTableProxy[] components = this.proxy.GetComponents<PlayMakerHashTableProxy>();
			PlayMakerHashTableProxy[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				PlayMakerHashTableProxy playMakerHashTableProxy = array[i];
				if (playMakerHashTableProxy.referenceName == this.reference.Value)
				{
					UnityEngine.Object.Destroy(playMakerHashTableProxy);
					base.Fsm.Event(this.successEvent);
					return;
				}
			}
			base.Fsm.Event(this.notFoundEvent);
		}
	}
}
