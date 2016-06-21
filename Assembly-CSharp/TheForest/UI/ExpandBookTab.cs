using System;
using UnityEngine;

namespace TheForest.UI
{
	public class ExpandBookTab : MonoBehaviour
	{
		public GameObject _tabNum;

		public GameObject _tabNum2;

		private Vector3 _expandedPos;

		private Vector3 _velocity;

		private SelectPageNumber _holder;

		private SelectPageNumber _self;

		private bool _registered;

		private void Awake()
		{
			this._expandedPos = base.transform.localPosition;
			base.transform.localPosition = Vector3.zero;
			this._holder = base.transform.parent.GetComponent<SelectPageNumber>();
			this._self = base.GetComponent<SelectPageNumber>();
			if (this._tabNum2)
			{
				this._tabNum2.SetActive(false);
			}
			this._tabNum.SetActive(false);
		}

		private void Update()
		{
			if (this._self.SelfOvered)
			{
				if (!this._registered)
				{
					this._registered = true;
					this._holder.BranchOvered++;
				}
			}
			else if (this._registered)
			{
				this._registered = false;
				this._holder.BranchOvered--;
			}
			base.transform.localPosition = Vector3.SmoothDamp(base.transform.localPosition, (!this._holder.IsOvered) ? Vector3.zero : this._expandedPos, ref this._velocity, 0.2f);
			if (this._tabNum && (this._holder.IsOvered && Vector3.Distance(base.transform.localPosition, this._expandedPos) < 0.01f) != this._tabNum.activeSelf)
			{
				if (this._tabNum2)
				{
					this._tabNum2.SetActive(!this._tabNum.activeSelf);
				}
				this._tabNum.SetActive(!this._tabNum.activeSelf);
			}
		}

		private void OnDisable()
		{
			base.transform.localPosition = Vector3.zero;
			this._registered = false;
		}
	}
}
