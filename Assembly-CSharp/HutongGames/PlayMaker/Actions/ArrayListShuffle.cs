using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), HutongGames.PlayMaker.Tooltip("Shuffle elements from an ArrayList Proxy component")]
	public class ArrayListShuffle : ArrayListActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, HutongGames.PlayMaker.Tooltip("The gameObject with the PlayMaker ArrayList Proxy component to shuffle")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component to copy from ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[HutongGames.PlayMaker.Tooltip("Optional start Index for the shuffling. Leave it to 0 for no effect")]
		public FsmInt startIndex;

		[HutongGames.PlayMaker.Tooltip("Optional range for the shuffling, starting at the start index if greater than 0. Leave it to 0 for no effect, that is will shuffle the whole array")]
		public FsmInt shufflingRange;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.startIndex = 0;
			this.shufflingRange = 0;
		}

		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoArrayListShuffle(this.proxy.arrayList);
			}
			base.Finish();
		}

		public void DoArrayListShuffle(ArrayList source)
		{
			if (!base.isProxyValid())
			{
				return;
			}
			int num = 0;
			int num2 = this.proxy.arrayList.Count - 1;
			if (this.startIndex.Value > 0)
			{
				num = Mathf.Min(this.startIndex.Value, num2);
			}
			if (this.shufflingRange.Value > 0)
			{
				num2 = Mathf.Min(this.proxy.arrayList.Count - 1, num + this.shufflingRange.Value);
			}
			Debug.Log(num);
			Debug.Log(num2);
			for (int i = num2; i > num; i--)
			{
				int index = UnityEngine.Random.Range(num, i + 1);
				object value = this.proxy.arrayList[i];
				this.proxy.arrayList[i] = this.proxy.arrayList[index];
				this.proxy.arrayList[index] = value;
			}
		}
	}
}
