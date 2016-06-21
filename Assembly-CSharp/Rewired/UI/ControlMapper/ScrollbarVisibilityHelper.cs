using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class ScrollbarVisibilityHelper : MonoBehaviour
	{
		public ScrollRect scrollRect;

		private bool onlySendMessage;

		private ScrollbarVisibilityHelper target;

		private Scrollbar hScrollBar
		{
			get
			{
				return (!(this.scrollRect != null)) ? null : this.scrollRect.horizontalScrollbar;
			}
		}

		private Scrollbar vScrollBar
		{
			get
			{
				return (!(this.scrollRect != null)) ? null : this.scrollRect.verticalScrollbar;
			}
		}

		private void Awake()
		{
			if (this.scrollRect != null)
			{
				this.target = this.scrollRect.gameObject.AddComponent<ScrollbarVisibilityHelper>();
				this.target.onlySendMessage = true;
				this.target.target = this;
			}
		}

		private void OnRectTransformDimensionsChange()
		{
			if (this.onlySendMessage)
			{
				if (this.target != null)
				{
					this.target.ScrollRectTransformDimensionsChanged();
				}
			}
			else
			{
				this.EvaluateScrollbar();
			}
		}

		private void ScrollRectTransformDimensionsChanged()
		{
			this.OnRectTransformDimensionsChange();
		}

		private void EvaluateScrollbar()
		{
			if (this.scrollRect == null)
			{
				return;
			}
			if (this.vScrollBar == null && this.hScrollBar == null)
			{
				return;
			}
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			Rect rect = this.scrollRect.content.rect;
			Rect rect2 = (this.scrollRect.transform as RectTransform).rect;
			if (this.vScrollBar != null)
			{
				bool value = rect.height > rect2.height;
				this.SetActiveDeferred(this.vScrollBar.gameObject, value);
			}
			if (this.hScrollBar != null)
			{
				bool value2 = rect.width > rect2.width;
				this.SetActiveDeferred(this.hScrollBar.gameObject, value2);
			}
		}

		private void SetActiveDeferred(GameObject obj, bool value)
		{
			base.StopAllCoroutines();
			base.StartCoroutine(this.SetActiveCoroutine(obj, value));
		}

		[DebuggerHidden]
		private IEnumerator SetActiveCoroutine(GameObject obj, bool value)
		{
			ScrollbarVisibilityHelper.<SetActiveCoroutine>c__Iterator112 <SetActiveCoroutine>c__Iterator = new ScrollbarVisibilityHelper.<SetActiveCoroutine>c__Iterator112();
			<SetActiveCoroutine>c__Iterator.obj = obj;
			<SetActiveCoroutine>c__Iterator.value = value;
			<SetActiveCoroutine>c__Iterator.<$>obj = obj;
			<SetActiveCoroutine>c__Iterator.<$>value = value;
			return <SetActiveCoroutine>c__Iterator;
		}
	}
}
