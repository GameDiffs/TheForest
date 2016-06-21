using System;
using TheForest.Items;
using TheForest.Items.Core;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Networking
{
	public class StealItemTrigger : MonoBehaviour
	{
		public GameObject _sheenIcon;

		public GameObject _pickupIcon;

		public BoltEntity _entity;

		[ItemIdPicker]
		public int[] _stealableItems;

		[ItemIdPicker]
		public int _metalTinTrayItemId;

		private float _nextSteal;

		private void Awake()
		{
			base.enabled = false;
		}

		private void OnEnable()
		{
			if (this._sheenIcon)
			{
				this._sheenIcon.SetActive(true);
				this._pickupIcon.SetActive(false);
			}
		}

		private void OnDisable()
		{
			if (this._sheenIcon)
			{
				this._sheenIcon.SetActive(this._pickupIcon.activeSelf && base.gameObject.activeSelf);
				this._pickupIcon.SetActive(false);
			}
		}

		private void OnDestroy()
		{
		}

		private void Update()
		{
			if (this._nextSteal < Time.realtimeSinceStartup)
			{
				if (TheForest.Utils.Input.GetButtonAfterDelay("Take", 0.5f))
				{
					this._nextSteal = Time.realtimeSinceStartup + 5f;
					LocalPlayer.Sfx.PlayWhoosh();
					if (this._entity.isAttached && this._entity.source != LocalPlayer.Entity.source)
					{
						StealItem stealItem = StealItem.Create(this._entity.source);
						stealItem.thief = LocalPlayer.Entity;
						stealItem.robbed = this._entity;
						stealItem.Send();
					}
					else
					{
						ItemStorage componentInParent = this._entity.GetComponentInParent<ItemStorage>();
						if (componentInParent)
						{
							bool useAltProjectile = LocalPlayer.Inventory.UseAltProjectile;
							LocalPlayer.Inventory.UseAltProjectile = true;
							for (int i = 0; i < componentInParent.UsedSlots.Count; i++)
							{
								LocalPlayer.Inventory.AddItem(componentInParent.UsedSlots[i]._itemId, componentInParent.UsedSlots[i]._amount, true, false, (WeaponStatUpgrade.Types)(-2));
								if (componentInParent.UsedSlots[i]._maxAmountBonus > 0)
								{
									LocalPlayer.Inventory.SetMaxAmountBonus(componentInParent.UsedSlots[i]._itemId, componentInParent.UsedSlots[i]._maxAmountBonus);
								}
							}
							if (this._entity.isAttached)
							{
								BoltNetwork.Destroy(componentInParent.gameObject);
							}
							else
							{
								UnityEngine.Object.Destroy(componentInParent.gameObject);
							}
							LocalPlayer.Inventory.UseAltProjectile = useAltProjectile;
						}
					}
					this._sheenIcon.SetActive(false);
					this._pickupIcon.SetActive(false);
					base.gameObject.SetActive(false);
				}
				else if (!this._pickupIcon.activeSelf)
				{
					this._sheenIcon.SetActive(false);
					this._pickupIcon.SetActive(true);
				}
			}
			else if (this._pickupIcon.activeSelf)
			{
				this._sheenIcon.SetActive(true);
				this._pickupIcon.SetActive(false);
			}
		}

		public void ActivateIfIsStealableItem(GameObject held)
		{
			HeldItemIdentifier component = held.GetComponent<HeldItemIdentifier>();
			if (component)
			{
				int itemId = component._itemId;
				if (itemId == this._metalTinTrayItemId)
				{
					this._nextSteal = Time.realtimeSinceStartup + 0.5f;
					base.gameObject.SetActive(true);
					return;
				}
				for (int i = 0; i < this._stealableItems.Length; i++)
				{
					if (this._stealableItems[i] == itemId)
					{
						this._nextSteal = Time.realtimeSinceStartup + 0.5f;
						base.gameObject.SetActive(true);
						return;
					}
				}
				this._sheenIcon.SetActive(false);
				this._pickupIcon.SetActive(false);
				base.gameObject.SetActive(false);
			}
		}

		private void GrabEnter()
		{
			base.enabled = true;
		}

		private void GrabExit()
		{
			base.enabled = false;
		}
	}
}
