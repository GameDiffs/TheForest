using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Rewired.UI.ControlMapper
{
	public interface ICustomSelectable : ICancelHandler, IEventSystemHandler
	{
		event UnityAction CancelEvent;

		Sprite disabledHighlightedSprite
		{
			get;
			set;
		}

		Color disabledHighlightedColor
		{
			get;
			set;
		}

		string disabledHighlightedTrigger
		{
			get;
			set;
		}

		bool autoNavUp
		{
			get;
			set;
		}

		bool autoNavDown
		{
			get;
			set;
		}

		bool autoNavLeft
		{
			get;
			set;
		}

		bool autoNavRight
		{
			get;
			set;
		}
	}
}
