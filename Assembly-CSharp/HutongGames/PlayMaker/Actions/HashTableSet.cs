using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable"), Tooltip("Set an key/value pair to a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy)")]
	public class HashTableSet : HashTableActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerHashTableProxy)), RequiredField, Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[RequiredField, Tooltip("The Key value for that hash set"), UIHint(UIHint.FsmString)]
		public FsmString key;

		[ActionSection("Result"), Tooltip("The variable to set.")]
		public FsmVar variable;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.key = null;
			this.variable = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.SetHashTable();
			}
			base.Finish();
		}

		public void SetHashTable()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.hashTable[this.key.Value] = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variable);
		}
	}
}
