using Bolt;
using System;
using TheForest.Items;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace TheForest.Networking
{
	public class RespawnDeadTrigger : MonoBehaviour
	{
		[ItemIdPicker]
		public int _healItemId;

		public GameObject _sheenIcon;

		public GameObject _pickupIcon;

		public float _maxKillDistance;

		public BoltEntity _entity;

		private void Awake()
		{
			base.enabled = false;
		}

		private void OnEnable()
		{
			this._sheenIcon.SetActive(true);
			this._pickupIcon.SetActive(false);
		}

		private void OnDisable()
		{
			LocalPlayer.Tuts.HideReviveMP();
		}

		private void OnDestroy()
		{
			LocalPlayer.Inventory.Attacked.AddListener(new UnityAction(this.LocalPlayerAttacked));
			LocalPlayer.Tuts.HideReviveMP();
		}

		private void Update()
		{
			if (TheForest.Utils.Input.GetButtonAfterDelay("Take", 2.5f))
			{
				LocalPlayer.Tuts.HideReviveMP();
				LocalPlayer.Sfx.PlayTwinkle();
				PlayerHealed playerHealed = PlayerHealed.Raise(GlobalTargets.Others);
				playerHealed.HealingItemId = this._healItemId;
				playerHealed.HealTarget = this._entity;
				playerHealed.Send();
				base.gameObject.SetActive(false);
			}
			else if (!this._pickupIcon.activeSelf)
			{
				this._sheenIcon.SetActive(false);
				this._pickupIcon.SetActive(true);
			}
		}

		private void GrabEnter()
		{
			LocalPlayer.Inventory.Attacked.AddListener(new UnityAction(this.LocalPlayerAttacked));
			if (!LocalPlayer.Inventory.Owns(this._healItemId))
			{
				LocalPlayer.Tuts.ShowReviveMP();
			}
			base.enabled = true;
		}

		private void GrabExit()
		{
			LocalPlayer.Inventory.Attacked.RemoveListener(new UnityAction(this.LocalPlayerAttacked));
			LocalPlayer.Tuts.HideReviveMP();
			base.enabled = false;
		}

		private void LocalPlayerAttacked()
		{
			if (!this || !base.transform || !LocalPlayer.Transform)
			{
				LocalPlayer.Tuts.HideReviveMP();
				LocalPlayer.Inventory.Attacked.RemoveListener(new UnityAction(this.LocalPlayerAttacked));
				return;
			}
			if (Vector3.Distance(base.transform.position, LocalPlayer.Transform.position) < this._maxKillDistance)
			{
				HitPlayer hitPlayer = HitPlayer.Raise(GlobalTargets.Others);
				hitPlayer.Target = this._entity;
				hitPlayer.Send();
				LocalPlayer.Inventory.Attacked.RemoveListener(new UnityAction(this.LocalPlayerAttacked));
				base.gameObject.SetActive(false);
				LocalPlayer.Sfx.PlayKillRabbit();
				LocalPlayer.Tuts.HideReviveMP();
			}
		}
	}
}
