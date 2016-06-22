using Rewired.UI.ControlMapper;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	public class ControlMapperDemoMessage : MonoBehaviour
	{
		public ControlMapper controlMapper;

		public Selectable defaultSelectable;

		private void Awake()
		{
			if (this.controlMapper != null)
			{
				this.controlMapper.ScreenClosedEvent += new Action(this.OnControlMapperClosed);
				this.controlMapper.ScreenOpenedEvent += new Action(this.OnControlMapperOpened);
			}
		}

		private void Start()
		{
			this.SelectDefault();
		}

		private void OnControlMapperClosed()
		{
			base.gameObject.SetActive(true);
			base.StartCoroutine(this.SelectDefaultDeferred());
		}

		private void OnControlMapperOpened()
		{
			base.gameObject.SetActive(false);
		}

		private void SelectDefault()
		{
			if (EventSystem.current == null)
			{
				return;
			}
			if (this.defaultSelectable != null)
			{
				EventSystem.current.SetSelectedGameObject(this.defaultSelectable.gameObject);
			}
		}

		[DebuggerHidden]
		private IEnumerator SelectDefaultDeferred()
		{
			ControlMapperDemoMessage.<SelectDefaultDeferred>c__Iterator110 <SelectDefaultDeferred>c__Iterator = new ControlMapperDemoMessage.<SelectDefaultDeferred>c__Iterator110();
			<SelectDefaultDeferred>c__Iterator.<>f__this = this;
			return <SelectDefaultDeferred>c__Iterator;
		}
	}
}
