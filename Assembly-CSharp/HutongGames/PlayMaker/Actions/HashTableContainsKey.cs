using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable"), Tooltip("Check if a key exists in a PlayMaker HashTable Proxy component (PlayMakerHashTablePRoxy)")]
	public class HashTableContainsKey : HashTableActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerHashTableProxy)), RequiredField, Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[RequiredField, Tooltip("The Key value to check for"), UIHint(UIHint.FsmString)]
		public FsmString key;

		[Tooltip("Store the result of the test"), UIHint(UIHint.FsmBool)]
		public FsmBool containsKey;

		[ActionSection("Result"), Tooltip("The event to trigger when key is found"), UIHint(UIHint.FsmEvent)]
		public FsmEvent keyFoundEvent;

		[Tooltip("The event to trigger when key is not found"), UIHint(UIHint.FsmEvent)]
		public FsmEvent keyNotFoundEvent;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.key = null;
			this.containsKey = null;
			this.keyFoundEvent = null;
			this.keyNotFoundEvent = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.checkIfContainsKey();
			}
			base.Finish();
		}

		public void checkIfContainsKey()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.containsKey.Value = this.proxy.hashTable.ContainsKey(this.key.Value);
			if (this.containsKey.Value)
			{
				base.Fsm.Event(this.keyFoundEvent);
			}
			else
			{
				base.Fsm.Event(this.keyNotFoundEvent);
			}
		}
	}
}
