using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable"), Tooltip("Gets items from a PlayMaker HashTable Proxy component")]
	public class HashTableGetMany : HashTableActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerHashTableProxy)), RequiredField, Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[ActionSection("Data"), CompoundArray("Count", "Key", "Value"), RequiredField, Tooltip("The Key value for that hash set"), UIHint(UIHint.FsmString)]
		public FsmString[] keys;

		[Tooltip("The value for that key"), UIHint(UIHint.Variable)]
		public FsmVar[] results;

		public override void Reset()
		{
			this.gameObject = null;
			this.keys = null;
			this.results = null;
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
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (this.proxy.hashTable.ContainsKey(this.keys[i].Value))
				{
					PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, this.results[i], this.proxy.hashTable[this.keys[i].Value]);
				}
			}
		}
	}
}
