using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), HutongGames.PlayMaker.Tooltip("Concat joins two or more arrayList proxy components. if a target is specified, the method use the target store the concatenation, else the ")]
	public class ArrayListConcat : ArrayListActions
	{
		[ActionSection("Storage"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, HutongGames.PlayMaker.Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component to store the concatenation ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[ActionSection("ArrayLists to concatenate"), CompoundArray("ArrayLists", "ArrayList GameObject", "Reference"), ObjectType(typeof(PlayMakerArrayListProxy)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject with the PlayMaker ArrayList Proxy component to copy to")]
		public FsmOwnerDefault[] arrayListGameObjectTargets;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component to copy to ( necessary if several component coexists on the same GameObject")]
		public FsmString[] referenceTargets;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.arrayListGameObjectTargets = null;
			this.referenceTargets = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoArrayListConcat(this.proxy.arrayList);
			}
			base.Finish();
		}

		public void DoArrayListConcat(ArrayList source)
		{
			if (!base.isProxyValid())
			{
				return;
			}
			for (int i = 0; i < this.arrayListGameObjectTargets.Length; i++)
			{
				if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.arrayListGameObjectTargets[i]), this.referenceTargets[i].Value) && base.isProxyValid())
				{
					foreach (object current in this.proxy.arrayList)
					{
						source.Add(current);
						Debug.Log("count " + source.Count);
					}
				}
			}
		}
	}
}
