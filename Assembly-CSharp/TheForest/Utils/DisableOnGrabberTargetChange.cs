using System;
using UnityEngine;

namespace TheForest.Utils
{
	public class DisableOnGrabberTargetChange : MonoBehaviour
	{
		private GameObject _target;

		private void OnEnable()
		{
			this._target = Grabber.FocusedItemGO;
		}

		private void Update()
		{
			if (this._target != Grabber.FocusedItemGO || !Grabber.FocusedItemGO)
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
