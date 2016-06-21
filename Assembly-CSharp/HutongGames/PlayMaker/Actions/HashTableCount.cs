using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable"), Tooltip("Count the number of items ( key/value pairs) in a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy).")]
	public class HashTableCount : HashTableActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerHashTableProxy)), RequiredField, Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[ActionSection("Result"), RequiredField, Tooltip("The number of items in that hashTable"), UIHint(UIHint.Variable)]
		public FsmInt count;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.count = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.doHashTableCount();
			}
			base.Finish();
		}

		public void doHashTableCount()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.count.Value = this.proxy.hashTable.Count;
		}
	}
}
