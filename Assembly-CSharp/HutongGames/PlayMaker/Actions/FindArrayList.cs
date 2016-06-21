using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), HutongGames.PlayMaker.Tooltip("Finds an ArrayList by reference. Warning: this function can be very slow.")]
	public class FindArrayList : CollectionsActions
	{
		[ActionSection("Set up"), RequiredField, HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component"), UIHint(UIHint.FsmString)]
		public FsmString ArrayListReference;

		[ActionSection("Result"), RequiredField, HutongGames.PlayMaker.Tooltip("Store the GameObject hosting the PlayMaker ArrayList Proxy component here")]
		public FsmGameObject store;

		public FsmEvent foundEvent;

		public FsmEvent notFoundEvent;

		public override void Reset()
		{
			this.ArrayListReference = string.Empty;
			this.store = null;
			this.foundEvent = null;
			this.notFoundEvent = null;
		}

		public override void OnEnter()
		{
			PlayMakerArrayListProxy[] array = UnityEngine.Object.FindObjectsOfType(typeof(PlayMakerArrayListProxy)) as PlayMakerArrayListProxy[];
			PlayMakerArrayListProxy[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				PlayMakerArrayListProxy playMakerArrayListProxy = array2[i];
				if (playMakerArrayListProxy.referenceName == this.ArrayListReference.Value)
				{
					this.store.Value = playMakerArrayListProxy.gameObject;
					base.Fsm.Event(this.foundEvent);
					return;
				}
			}
			this.store.Value = null;
			base.Fsm.Event(this.notFoundEvent);
			base.Finish();
		}
	}
}
