using System;
using UnityEngine;

namespace TheForest.UI
{
	public class TogglePosition : MonoBehaviour
	{
		public UIWidget _target;

		private Vector3 _originalPosition;

		private UIRect.AnchorUpdate _anchorUpdate;

		private void OnEnable()
		{
			this._originalPosition = this._target.transform.position;
			this._target.transform.position = base.transform.position;
			this._anchorUpdate = this._target.updateAnchors;
			this._target.updateAnchors = UIRect.AnchorUpdate.OnStart;
			this._target.GetComponentInParent<UIPanel>().Refresh();
		}

		private void OnDisable()
		{
			this._target.transform.position = this._originalPosition;
			this._target.updateAnchors = this._anchorUpdate;
		}
	}
}
