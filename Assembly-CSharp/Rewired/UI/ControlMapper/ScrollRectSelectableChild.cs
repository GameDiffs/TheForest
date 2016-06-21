using Rewired.Utils;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu(""), RequireComponent(typeof(Selectable))]
	public class ScrollRectSelectableChild : MonoBehaviour, IEventSystemHandler, ISelectHandler
	{
		public bool useCustomEdgePadding;

		public float customEdgePadding = 50f;

		private ScrollRect parentScrollRect;

		private Selectable _selectable;

		private RectTransform parentScrollRectContentTransform
		{
			get
			{
				return this.parentScrollRect.content;
			}
		}

		private Selectable selectable
		{
			get
			{
				Selectable arg_1C_0;
				if ((arg_1C_0 = this._selectable) == null)
				{
					arg_1C_0 = (this._selectable = base.GetComponent<Selectable>());
				}
				return arg_1C_0;
			}
		}

		private RectTransform rectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		private void Start()
		{
			this.parentScrollRect = base.transform.GetComponentInParent<ScrollRect>();
			if (this.parentScrollRect == null)
			{
				Debug.LogError("Rewired Control Mapper: No ScrollRect found! This component must be a child of a ScrollRect!");
				return;
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			if (this.parentScrollRect == null)
			{
				return;
			}
			if (!(eventData is AxisEventData))
			{
				return;
			}
			RectTransform rectTransform = this.parentScrollRect.transform as RectTransform;
			Rect child = MathTools.TransformRect(this.rectTransform.rect, this.rectTransform, rectTransform);
			Rect rect = rectTransform.rect;
			Rect rect2 = rectTransform.rect;
			float height;
			if (this.useCustomEdgePadding)
			{
				height = this.customEdgePadding;
			}
			else
			{
				height = child.height;
			}
			rect2.yMax -= height;
			rect2.yMin += height;
			if (MathTools.RectContains(rect2, child))
			{
				return;
			}
			Vector2 vector;
			if (!MathTools.GetOffsetToContainRect(rect2, child, out vector))
			{
				return;
			}
			Vector2 anchoredPosition = this.parentScrollRectContentTransform.anchoredPosition;
			anchoredPosition.x = Mathf.Clamp(anchoredPosition.x + vector.x, 0f, Mathf.Abs(rect.width - this.parentScrollRectContentTransform.sizeDelta.x));
			anchoredPosition.y = Mathf.Clamp(anchoredPosition.y + vector.y, 0f, Mathf.Abs(rect.height - this.parentScrollRectContentTransform.sizeDelta.y));
			this.parentScrollRectContentTransform.anchoredPosition = anchoredPosition;
		}
	}
}
