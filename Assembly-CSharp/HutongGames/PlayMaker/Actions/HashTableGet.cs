using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable"), Tooltip("Gets an item from a PlayMaker HashTable Proxy component")]
	public class HashTableGet : HashTableActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerHashTableProxy)), RequiredField, Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[RequiredField, Tooltip("The Key value for that hash set"), UIHint(UIHint.FsmString)]
		public FsmString key;

		[ActionSection("Result"), UIHint(UIHint.Variable)]
		public FsmVar result;

		[Tooltip("The event to trigger when key is found"), UIHint(UIHint.FsmEvent)]
		public FsmEvent KeyFoundEvent;

		[Tooltip("The event to trigger when key is not found"), UIHint(UIHint.FsmEvent)]
		public FsmEvent KeyNotFoundEvent;

		public override void Reset()
		{
			this.gameObject = null;
			this.key = null;
			this.KeyFoundEvent = null;
			this.KeyNotFoundEvent = null;
			this.result = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.Get();
			}
			base.Finish();
		}

		public void Get()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			if (!this.proxy.hashTable.ContainsKey(this.key.Value))
			{
				base.Fsm.Event(this.KeyNotFoundEvent);
				return;
			}
			PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, this.result, this.proxy.hashTable[this.key.Value]);
			base.Fsm.Event(this.KeyFoundEvent);
		}
	}
}
