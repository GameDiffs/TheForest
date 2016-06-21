using Bolt;
using System;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/World/Treesap Source")]
	public class TreesapSource : EntityBehaviour<IWaterSourceState>
	{
		[SerializeThis]
		public int _amount;

		public float _maxAmount = 30f;

		public GameObject _billboardGatherPickup;

		public GameObject _billboardGatherSheen;

		public Transform _fillRenderer;

		public Transform _fillRendererMin;

		public Transform _fillRendererMax;

		[ItemIdPicker]
		public int _treesapItemId;

		private bool CanGather
		{
			get
			{
				return this._amount > 0;
			}
		}

		private void Awake()
		{
			base.InvokeRepeating("AddRandomAmount", 300f, 300f);
			this._billboardGatherPickup.SetActive(false);
			this._billboardGatherSheen.SetActive(false);
			base.enabled = false;
		}

		private void Update()
		{
			bool canGather = this.CanGather;
			if (canGather && TheForest.Utils.Input.GetButtonDown("Take"))
			{
				LocalPlayer.Sfx.PlayTwinkle();
				LocalPlayer.Inventory.AddItem(this._treesapItemId, this._amount, false, false, (WeaponStatUpgrade.Types)(-2));
				this.RemoveTreesap((float)this._amount);
			}
			this.ToggleIcons(true);
		}

		private void OnDeserialized()
		{
			this.UpdateRenderer();
			this.ToggleIcons(false);
		}

		private void GrabEnter()
		{
			base.enabled = true;
			this.ToggleIcons(true);
		}

		private void GrabExit()
		{
			base.enabled = false;
			this.ToggleIcons(false);
		}

		public void AddRandomAmount()
		{
			int num = UnityEngine.Random.Range(0, 4);
			if (num > 0)
			{
				this.AddTreesap((float)num);
			}
		}

		public void AddTreesap(float amount)
		{
			this._amount = (int)Mathf.Min((float)this._amount + amount, this._maxAmount);
			if (BoltNetwork.isRunning && this.entity && this.entity.isAttached)
			{
				base.state.amount = (float)this._amount;
			}
			this.ToggleIcons(base.enabled);
			this.UpdateRenderer();
		}

		public void RemoveTreesap(float amount)
		{
			this._amount = (int)Mathf.Max((float)this._amount - amount, 0f);
			if (BoltNetwork.isRunning && this.entity && this.entity.isAttached)
			{
				base.state.amount = (float)this._amount;
			}
			this.ToggleIcons(base.enabled);
			this.UpdateRenderer();
		}

		private void UpdateRenderer()
		{
			if (this._amount > 0)
			{
				if (!this._fillRenderer.gameObject.activeSelf)
				{
					this._fillRenderer.gameObject.SetActive(true);
				}
				float t = (float)this._amount / this._maxAmount;
				this._fillRenderer.position = Vector3.Lerp(this._fillRendererMin.position, this._fillRendererMax.position, t);
				this._fillRenderer.localScale = Vector3.Lerp(this._fillRendererMin.localScale, this._fillRendererMax.localScale, t);
			}
			else
			{
				this._fillRenderer.gameObject.SetActive(false);
			}
		}

		private void ToggleIcons(bool pickup)
		{
			bool canGather = this.CanGather;
			if (pickup)
			{
				this._billboardGatherPickup.SetActive(canGather);
				this._billboardGatherSheen.SetActive(false);
			}
			else
			{
				this._billboardGatherPickup.SetActive(false);
				this._billboardGatherSheen.SetActive(canGather);
			}
		}

		public override void Attached()
		{
			if (BoltNetwork.isServer)
			{
				base.state.amount = (float)this._amount;
			}
			base.state.AddCallback("amount", delegate
			{
				this._amount = (int)base.state.amount;
			});
		}
	}
}
