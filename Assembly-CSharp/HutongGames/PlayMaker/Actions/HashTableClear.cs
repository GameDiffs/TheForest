using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable"), Tooltip("Remove all content of a PlayMaker hashtable Proxy component")]
	public class HashTableClear : HashTableActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerHashTableProxy)), RequiredField, Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.ClearHashTable();
			}
			base.Finish();
		}

		public void ClearHashTable()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.hashTable.Clear();
		}
	}
}
