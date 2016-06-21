using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable"), HutongGames.PlayMaker.Tooltip("Adds a PlayMakerHashTableProxy Component to a Game Object. Use this to create arrayList on the fly instead of during authoring.\n Optionally remove the HashTable component on exiting the state.\n Simply point to existing if the reference exists already.")]
	public class HashTableCreate : HashTableActions
	{
		[ActionSection("Set up"), RequiredField, HutongGames.PlayMaker.Tooltip("The Game Object to add the Hashtable Component to.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker arrayList proxy component ( necessary if several component coexists on the same GameObject"), UIHint(UIHint.FsmString)]
		public FsmString reference;

		[HutongGames.PlayMaker.Tooltip("Remove the Component when this State is exited.")]
		public FsmBool removeOnExit;

		[ActionSection("Result"), HutongGames.PlayMaker.Tooltip("The event to trigger if the hashtable exists already"), UIHint(UIHint.FsmEvent)]
		public FsmEvent alreadyExistsEvent;

		private PlayMakerHashTableProxy addedComponent;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.alreadyExistsEvent = null;
		}

		public override void OnEnter()
		{
			this.DoAddPlayMakerHashTable();
			base.Finish();
		}

		public override void OnExit()
		{
			if (this.removeOnExit.Value && this.addedComponent != null)
			{
				UnityEngine.Object.Destroy(this.addedComponent);
			}
		}

		private void DoAddPlayMakerHashTable()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			PlayMakerHashTableProxy hashTableProxyPointer = base.GetHashTableProxyPointer(ownerDefaultTarget, this.reference.Value, true);
			if (hashTableProxyPointer != null)
			{
				base.Fsm.Event(this.alreadyExistsEvent);
			}
			else
			{
				this.addedComponent = ownerDefaultTarget.AddComponent<PlayMakerHashTableProxy>();
				if (this.addedComponent == null)
				{
					Debug.LogError("Can't add PlayMakerHashTableProxy");
				}
				else
				{
					this.addedComponent.referenceName = this.reference.Value;
				}
			}
		}
	}
}
