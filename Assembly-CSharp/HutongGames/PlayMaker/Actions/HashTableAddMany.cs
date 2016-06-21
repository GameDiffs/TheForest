using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable"), Tooltip("Add key/value pairs to a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy).")]
	public class HashTableAddMany : HashTableActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerHashTableProxy)), RequiredField, Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[ActionSection("Data"), CompoundArray("Count", "Key", "Value"), RequiredField, Tooltip("The Key"), UIHint(UIHint.FsmString)]
		public FsmString[] keys;

		[RequiredField, Tooltip("The value for that key")]
		public FsmVar[] variables;

		[ActionSection("Result"), Tooltip("The event to trigger when elements are added"), UIHint(UIHint.FsmEvent)]
		public FsmEvent successEvent;

		[Tooltip("The event to trigger when elements exists already"), UIHint(UIHint.FsmEvent)]
		public FsmEvent keyExistsAlreadyEvent;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.keys = null;
			this.variables = null;
			this.successEvent = null;
			this.keyExistsAlreadyEvent = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				if (this.keyExistsAlreadyEvent != null)
				{
					FsmString[] array = this.keys;
					for (int i = 0; i < array.Length; i++)
					{
						FsmString fsmString = array[i];
						if (this.proxy.hashTable.ContainsKey(fsmString.Value))
						{
							base.Fsm.Event(this.keyExistsAlreadyEvent);
							base.Finish();
						}
					}
				}
				this.AddToHashTable();
				base.Fsm.Event(this.successEvent);
			}
			base.Finish();
		}

		public void AddToHashTable()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			for (int i = 0; i < this.keys.Length; i++)
			{
				this.proxy.hashTable.Add(this.keys[i].Value, PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variables[i]));
			}
		}
	}
}
