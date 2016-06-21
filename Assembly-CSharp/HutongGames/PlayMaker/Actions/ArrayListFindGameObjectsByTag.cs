using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), HutongGames.PlayMaker.Tooltip("Store all active GameObjects with a specific tag. Tags must be declared in the tag manager before using them")]
	public class ArrayListFindGameObjectsByTag : ArrayListActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, HutongGames.PlayMaker.Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[HutongGames.PlayMaker.Tooltip("the tag")]
		public FsmString tag;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.tag = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.FindGOByTag();
			}
			base.Finish();
		}

		public void FindGOByTag()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.arrayList.Clear();
			GameObject[] c = GameObject.FindGameObjectsWithTag(this.tag.Value);
			this.proxy.arrayList.InsertRange(0, c);
		}
	}
}
