using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), HutongGames.PlayMaker.Tooltip("Each time this action is called it gets the previous item from a PlayMaker ArrayList Proxy component. \nThis lets you quickly loop backward through all the children of an object to perform actions on them.\nNOTE: To get to specific item use ArrayListGet instead.")]
	public class ArrayListGetPrevious : ArrayListActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, HutongGames.PlayMaker.Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[HutongGames.PlayMaker.Tooltip("From where to start iteration, leave to 0 to start from the end. index is relative to the last item, so if the start index is 2, this will start 2 items before the last item.")]
		public FsmInt startIndex;

		[HutongGames.PlayMaker.Tooltip("When to end iteration, leave to 0 to iterate until the beginning, index is relative to the last item.")]
		public FsmInt endIndex;

		[HutongGames.PlayMaker.Tooltip("Event to send to get the next item.")]
		public FsmEvent loopEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send when there are no more items.")]
		public FsmEvent finishedEvent;

		[HutongGames.PlayMaker.Tooltip("The event to trigger if the action fails ( likely and index is out of range exception)"), UIHint(UIHint.FsmEvent)]
		public FsmEvent failureEvent;

		[ActionSection("Result"), UIHint(UIHint.Variable)]
		public FsmInt currentIndex;

		[UIHint(UIHint.Variable)]
		public FsmVar result;

		private int nextItemIndex;

		private int countBase;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.startIndex = null;
			this.endIndex = null;
			this.loopEvent = null;
			this.finishedEvent = null;
			this.failureEvent = null;
			this.currentIndex = null;
			this.result = null;
		}

		public override void OnEnter()
		{
			if (this.nextItemIndex == 0)
			{
				if (!base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
				{
					base.Fsm.Event(this.failureEvent);
					base.Finish();
				}
				this.countBase = this.proxy.arrayList.Count - 1;
				if (this.startIndex.Value > 0)
				{
					this.nextItemIndex = this.startIndex.Value;
				}
			}
			this.DoGetPreviousItem();
			base.Finish();
		}

		private void DoGetPreviousItem()
		{
			if (this.nextItemIndex > this.countBase)
			{
				this.nextItemIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			this.GetItemAtIndex();
			if (this.nextItemIndex >= this.countBase)
			{
				this.nextItemIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			if (this.endIndex.Value > 0 && this.nextItemIndex > this.countBase - this.endIndex.Value)
			{
				this.nextItemIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			this.nextItemIndex++;
			if (this.loopEvent != null)
			{
				base.Fsm.Event(this.loopEvent);
			}
		}

		public void GetItemAtIndex()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.currentIndex.Value = this.countBase - this.nextItemIndex;
			object value = null;
			try
			{
				value = this.proxy.arrayList[this.currentIndex.Value];
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				base.Fsm.Event(this.failureEvent);
				return;
			}
			PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, this.result, value);
		}
	}
}
