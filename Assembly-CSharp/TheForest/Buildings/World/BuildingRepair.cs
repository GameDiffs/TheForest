using System;
using TheForest.Buildings.Interfaces;
using TheForest.Items;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	public class BuildingRepair : MonoBehaviour
	{
		public IRepairableStructure _target;

		public GUITexture _icon;

		public GUIText _text;

		public GUITexture _iconLog;

		public GUIText _textLog;

		public GameObject _iconSheen;

		public GameObject _iconSheen2;

		public GameObject _iconPickUp;

		public GameObject _iconPickUp2;

		public Color _white;

		public Color _red;

		[ItemIdPicker]
		public int _itemId;

		[ItemIdPicker]
		public int _logItemId;

		private Vector3 _iconPos;

		private Vector3 _iconLogPos;

		private int _displayedLogCount;

		private int _displayedTotalLogCount;

		private int _displayedSapCount;

		private int _displayedTotalSapCount;

		private void Awake()
		{
			this._iconPos = this._icon.transform.localPosition;
			this._icon.gameObject.SetActive(false);
			this._iconLogPos = this._iconLog.transform.localPosition;
			this._iconLog.gameObject.SetActive(false);
			this._iconSheen.SetActive(true);
			this._iconPickUp.SetActive(false);
			base.enabled = false;
		}

		private void Update()
		{
			if (this._target != null)
			{
				bool flag = this._target.CalcMissingRepairLogs() > 0;
				bool flag2 = flag && LocalPlayer.Inventory.Owns(this._logItemId);
				bool flag3 = this._target.CalcMissingRepairMaterial() > 0;
				bool flag4 = flag3 && LocalPlayer.Inventory.Owns(this._itemId);
				if (flag)
				{
					this._iconLog.color = ((!flag2) ? this._red : this._white);
				}
				else if (this._iconLog.gameObject.activeSelf)
				{
					this._iconLog.gameObject.SetActive(false);
				}
				if (flag3)
				{
					this._icon.color = ((!flag4) ? this._red : this._white);
				}
				else if (this._icon.gameObject.activeSelf)
				{
					this._icon.gameObject.SetActive(false);
				}
				if ((flag2 || flag4) && TheForest.Utils.Input.GetButtonDown("Take"))
				{
					bool isLog;
					if (flag2)
					{
						LocalPlayer.Inventory.RemoveItem(this._logItemId, 1, false);
						isLog = true;
					}
					else
					{
						if (!flag4)
						{
							Debug.LogError(string.Concat(new object[]
							{
								"Repair system error, please report (canAddLog=",
								flag2,
								", canAddRepairMat=",
								flag4,
								")"
							}));
							return;
						}
						LocalPlayer.Inventory.RemoveItem(this._itemId, 1, false);
						isLog = false;
					}
					LocalPlayer.Sfx.PlayWhoosh();
					this._target.AddRepairMaterial(isLog);
				}
				this.UpdateLogDisplay();
				this.UpdateSapDisplay();
			}
			else
			{
				base.enabled = false;
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void GrabEnter()
		{
			int num = this._target.CalcTotalRepairMaterial();
			int collapsedLogs = this._target.CollapsedLogs;
			if (num > 0)
			{
				this.UpdateSapDisplay();
				this._icon.transform.position = this._iconPos;
				this._icon.gameObject.SetActive(true);
			}
			if (collapsedLogs > 0)
			{
				this.UpdateLogDisplay();
				this._iconLog.transform.position = this._iconLogPos;
				this._iconLog.gameObject.SetActive(true);
			}
			this._iconSheen.SetActive(false);
			this._iconPickUp.SetActive(true);
			base.enabled = true;
		}

		private void GrabExit()
		{
			this._icon.gameObject.SetActive(false);
			this._iconLog.gameObject.SetActive(false);
			this._iconSheen.SetActive(true);
			this._iconPickUp.SetActive(false);
			base.enabled = false;
		}

		public void UpdateLogDisplay()
		{
			int repairLogs = this._target.RepairLogs;
			int collapsedLogs = this._target.CollapsedLogs;
			if (this._displayedLogCount != repairLogs || this._displayedTotalLogCount != collapsedLogs)
			{
				this._textLog.text = this._target.RepairLogs + "/" + this._target.CollapsedLogs;
				this._displayedLogCount = repairLogs;
				this._displayedTotalLogCount = collapsedLogs;
			}
		}

		public void UpdateSapDisplay()
		{
			int repairMaterial = this._target.RepairMaterial;
			int num = this._target.CalcTotalRepairMaterial();
			if (this._displayedSapCount != repairMaterial || this._displayedTotalSapCount != num)
			{
				this._text.text = repairMaterial + "/" + num;
				this._displayedSapCount = repairMaterial;
				this._displayedTotalSapCount = num;
			}
		}
	}
}
