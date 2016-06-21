using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public abstract class UIElementInfo : MonoBehaviour, IEventSystemHandler, ISelectHandler
	{
		public string identifier;

		public int intData;

		public Text text;

		public event Action<GameObject> OnSelectedEvent
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.OnSelectedEvent = (Action<GameObject>)Delegate.Combine(this.OnSelectedEvent, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.OnSelectedEvent = (Action<GameObject>)Delegate.Remove(this.OnSelectedEvent, value);
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			if (this.OnSelectedEvent != null)
			{
				this.OnSelectedEvent(base.gameObject);
			}
		}
	}
}
