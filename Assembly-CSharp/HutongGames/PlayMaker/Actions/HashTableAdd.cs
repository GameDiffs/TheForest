using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable"), Tooltip("Add an key/value pair to a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy).")]
	public class HashTableAdd : HashTableActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerHashTableProxy)), RequiredField, Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[ActionSection("Data"), RequiredField, Tooltip("The Key value for that hash set"), UIHint(UIHint.FsmString)]
		public FsmString key;

		[RequiredField, Tooltip("The variable to add.")]
		public FsmVar variable;

		[ActionSection("Result"), Tooltip("The event to trigger when element is added"), UIHint(UIHint.FsmEvent)]
		public FsmEvent successEvent;

		[Tooltip("The event to trigger when element exists already"), UIHint(UIHint.FsmEvent)]
		public FsmEvent keyExistsAlreadyEvent;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.key = null;
			this.variable = null;
			this.successEvent = null;
			this.keyExistsAlreadyEvent = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				if (this.proxy.hashTable.ContainsKey(this.key.Value))
				{
					base.Fsm.Event(this.keyExistsAlreadyEvent);
				}
				else
				{
					this.AddToHashTable();
					base.Fsm.Event(this.successEvent);
				}
			}
			base.Finish();
		}

		public void AddToHashTable()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.hashTable.Add(this.key.Value, PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variable));
		}
	}
}
