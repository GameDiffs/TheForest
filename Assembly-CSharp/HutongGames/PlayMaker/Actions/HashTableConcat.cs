using System;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable"), Tooltip("Concat joins two or more hashTable proxy components. if a target is specified, the method use the target store the concatenation, else the ")]
	public class HashTableConcat : HashTableActions
	{
		[ActionSection("Storage"), CheckForComponent(typeof(PlayMakerHashTableProxy)), RequiredField, Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component to store the concatenation ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[ActionSection("HashTables to concatenate"), CompoundArray("HashTables", "HashTable GameObject", "Reference"), ObjectType(typeof(PlayMakerHashTableProxy)), RequiredField, Tooltip("The GameObject with the PlayMaker HashTable Proxy component to copy to")]
		public FsmOwnerDefault[] hashTableGameObjectTargets;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component to copy to ( necessary if several component coexists on the same GameObject")]
		public FsmString[] referenceTargets;

		[Tooltip("Overwrite existing key with new values"), UIHint(UIHint.FsmBool)]
		public FsmBool overwriteExistingKey;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.hashTableGameObjectTargets = null;
			this.referenceTargets = null;
			this.overwriteExistingKey = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoHashTableConcat(this.proxy.hashTable);
			}
			base.Finish();
		}

		public void DoHashTableConcat(Hashtable source)
		{
			if (!base.isProxyValid())
			{
				return;
			}
			for (int i = 0; i < this.hashTableGameObjectTargets.Length; i++)
			{
				if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.hashTableGameObjectTargets[i]), this.referenceTargets[i].Value) && base.isProxyValid())
				{
					foreach (object current in this.proxy.hashTable.Keys)
					{
						if (source.ContainsKey(current))
						{
							if (this.overwriteExistingKey.Value)
							{
								source[current] = this.proxy.hashTable[current];
							}
						}
						else
						{
							source[current] = this.proxy.hashTable[current];
						}
					}
				}
			}
		}
	}
}
