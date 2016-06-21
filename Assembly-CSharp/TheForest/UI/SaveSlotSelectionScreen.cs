using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TheForest.UI
{
	public class SaveSlotSelectionScreen : MonoBehaviour
	{
		public GameObject[] _disableWhenActive;

		public List<GameObject> _enableWhenInactive = new List<GameObject>();

		public UIButton[] _saveSlotsButtons;

		public static UnityEvent OnSlotSelected = new UnityEvent();

		public static UnityEvent OnSlotCanceled = new UnityEvent();

		private void OnEnable()
		{
			if (this._disableWhenActive.Length > 0)
			{
				GameObject[] disableWhenActive = this._disableWhenActive;
				for (int i = 0; i < disableWhenActive.Length; i++)
				{
					GameObject gameObject = disableWhenActive[i];
					if (gameObject.activeSelf)
					{
						this._enableWhenInactive.Add(gameObject);
						gameObject.SetActive(false);
					}
				}
			}
		}

		private void Update()
		{
			if (Cheats.PermaDeath && TitleScreen.StartGameSetup.Type == TitleScreen.GameSetup.InitTypes.Continue)
			{
				for (int i = 0; i < this._saveSlotsButtons.Length; i++)
				{
					this._saveSlotsButtons[i].isEnabled = (i + TitleScreen.GameSetup.Slots.Slot1 == TitleScreen.StartGameSetup.Slot);
				}
			}
		}

		private void OnDisable()
		{
			if (this._disableWhenActive.Length > 0)
			{
				foreach (GameObject current in this._enableWhenInactive)
				{
					current.SetActive(true);
				}
			}
			this._enableWhenInactive.Clear();
		}

		public void OnSlotSelection(TitleScreen.GameSetup.Slots slotNum)
		{
			TitleScreen.StartGameSetup.Type = TitleScreen.GameSetup.InitTypes.Continue;
			TitleScreen.StartGameSetup.Slot = slotNum;
			SaveSlotSelectionScreen.OnSlotSelected.Invoke();
		}

		public void OnCancel()
		{
			SaveSlotSelectionScreen.OnSlotCanceled.Invoke();
		}
	}
}
