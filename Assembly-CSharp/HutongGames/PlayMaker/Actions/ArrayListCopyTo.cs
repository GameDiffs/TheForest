using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), HutongGames.PlayMaker.Tooltip("Copy elements from one PlayMaker ArrayList Proxy component to another")]
	public class ArrayListCopyTo : ArrayListActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, HutongGames.PlayMaker.Tooltip("The gameObject with the PlayMaker ArrayList Proxy component to copy from")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component to copy from ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[ActionSection("Result"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, HutongGames.PlayMaker.Tooltip("The gameObject with the PlayMaker ArrayList Proxy component to copy to")]
		public FsmOwnerDefault gameObjectTarget;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component to copy to ( necessary if several component coexists on the same GameObject")]
		public FsmString referenceTarget;

		[HutongGames.PlayMaker.Tooltip("Optional start index to copy from the source, if not set, starts from the beginning")]
		public FsmInt startIndex;

		[HutongGames.PlayMaker.Tooltip("Optional amount of elements to copy, If not set, will copy all from start index.")]
		public FsmInt count;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.gameObjectTarget = null;
			this.referenceTarget = null;
			this.startIndex = new FsmInt
			{
				UseVariable = true
			};
			this.count = new FsmInt
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoArrayListCopyTo(this.proxy.arrayList);
			}
			base.Finish();
		}

		public void DoArrayListCopyTo(ArrayList source)
		{
			if (!base.isProxyValid())
			{
				return;
			}
			if (!base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObjectTarget), this.referenceTarget.Value))
			{
				return;
			}
			if (!base.isProxyValid())
			{
				return;
			}
			int value = this.startIndex.Value;
			int num = source.Count;
			int value2 = source.Count;
			if (!this.count.IsNone)
			{
				value2 = this.count.Value;
			}
			if (value < 0 || value >= source.Count)
			{
				this.LogError("start index out of range");
				return;
			}
			if (this.count.Value < 0)
			{
				this.LogError("count can not be negative");
				return;
			}
			num = Mathf.Min(value + value2, source.Count);
			for (int i = value; i < num; i++)
			{
				this.proxy.arrayList.Add(source[i]);
			}
		}
	}
}
