using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu(""), RequireComponent(typeof(Image))]
	public class UIImageHelper : MonoBehaviour
	{
		[Serializable]
		private class State
		{
			[SerializeField]
			public Color color;

			public void Set(Image image)
			{
				if (image == null)
				{
					return;
				}
				image.color = this.color;
			}
		}

		[SerializeField]
		private UIImageHelper.State enabledState;

		[SerializeField]
		private UIImageHelper.State disabledState;

		private bool currentState;

		public void SetEnabledState(bool newState)
		{
			this.currentState = newState;
			UIImageHelper.State state = (!newState) ? this.disabledState : this.enabledState;
			if (state == null)
			{
				return;
			}
			Image component = base.gameObject.GetComponent<Image>();
			if (component == null)
			{
				Debug.LogError("Image is missing!");
				return;
			}
			state.Set(component);
		}

		public void SetEnabledStateColor(Color color)
		{
			this.enabledState.color = color;
		}

		public void SetDisabledStateColor(Color color)
		{
			this.disabledState.color = color;
		}

		public void Refresh()
		{
			UIImageHelper.State state = (!this.currentState) ? this.disabledState : this.enabledState;
			Image component = base.gameObject.GetComponent<Image>();
			if (component == null)
			{
				return;
			}
			state.Set(component);
		}
	}
}
