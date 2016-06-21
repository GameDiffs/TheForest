using Bolt;
using System;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace TheForest.Player
{
	public class WeaponBonus : MonoBehaviour
	{
		public enum EventMode
		{
			AttackToAttackEnd,
			Released,
			Passive
		}

		public enum BonusTypes
		{
			Burn,
			Poison,
			DouseBurn
		}

		public WeaponBonus.EventMode _mode;

		public Transform _owner;

		public WeaponBonus.BonusTypes _bonusType;

		private void Start()
		{
			if (this._mode == WeaponBonus.EventMode.AttackToAttackEnd)
			{
				base.GetComponent<Collider>().enabled = false;
				LocalPlayer.Inventory.Attacked.AddListener(new UnityAction(this.OnAttack));
				LocalPlayer.Inventory.AttackEnded.AddListener(new UnityAction(this.EndAttack));
			}
			else if (this._mode == WeaponBonus.EventMode.Released)
			{
				base.GetComponent<Collider>().enabled = false;
				LocalPlayer.Inventory.ReleasedAttack.AddListener(new UnityAction(this.OnAttack));
			}
			else
			{
				base.GetComponent<Collider>().enabled = true;
			}
			if (!this._owner)
			{
				this._owner = base.transform.root;
			}
		}

		private void OnDestroy()
		{
			if (this._mode == WeaponBonus.EventMode.AttackToAttackEnd)
			{
				LocalPlayer.Inventory.Attacked.RemoveListener(new UnityAction(this.OnAttack));
				LocalPlayer.Inventory.AttackEnded.RemoveListener(new UnityAction(this.EndAttack));
			}
			else if (this._mode == WeaponBonus.EventMode.Released)
			{
				LocalPlayer.Inventory.ReleasedAttack.RemoveListener(new UnityAction(this.OnAttack));
			}
		}

		private void OnTriggerEnter(Collider otherObject)
		{
			if (this._owner && this._owner != otherObject.transform.root)
			{
				if (BoltNetwork.isRunning)
				{
					GameObject gameObject = otherObject.transform.root.gameObject;
					BoltEntity component = gameObject.GetComponent<BoltEntity>();
					if (component)
					{
						switch (this._bonusType)
						{
						case WeaponBonus.BonusTypes.Burn:
						{
							Burn burn = Burn.Create(GlobalTargets.OnlyServer);
							burn.Entity = component;
							burn.Send();
							break;
						}
						case WeaponBonus.BonusTypes.Poison:
							if (Vector3.Dot(otherObject.transform.position - base.transform.position, base.transform.forward) > 0.25f)
							{
								Poison poison = Poison.Create(GlobalTargets.OnlyServer);
								poison.Entity = component;
								poison.Send();
							}
							break;
						case WeaponBonus.BonusTypes.DouseBurn:
						{
							Burn burn2 = Burn.Create(GlobalTargets.OnlyServer);
							burn2.Entity = component;
							burn2.Send();
							break;
						}
						}
					}
				}
				switch (this._bonusType)
				{
				case WeaponBonus.BonusTypes.Burn:
					Prefabs.Instance.SpawnFireHitPS(base.transform.position, Quaternion.LookRotation(base.transform.position - otherObject.transform.position));
					otherObject.SendMessage("Burn", SendMessageOptions.DontRequireReceiver);
					break;
				case WeaponBonus.BonusTypes.Poison:
					if (Vector3.Dot(otherObject.transform.position - base.transform.position, base.transform.forward) > 0.25f)
					{
						otherObject.SendMessage("Poison", SendMessageOptions.DontRequireReceiver);
					}
					break;
				case WeaponBonus.BonusTypes.DouseBurn:
					Prefabs.Instance.SpawnFireHitPS(base.transform.position, Quaternion.LookRotation(base.transform.position - otherObject.transform.position));
					otherObject.SendMessage("Douse", SendMessageOptions.DontRequireReceiver);
					otherObject.SendMessage("Burn", SendMessageOptions.DontRequireReceiver);
					break;
				}
			}
		}

		private void OnAttack()
		{
			base.GetComponent<Collider>().enabled = true;
		}

		private void EndAttack()
		{
			base.GetComponent<Collider>().enabled = false;
		}
	}
}
