using Bolt;
using System;
using TheForest.Networking;
using UnityEngine;

public class CoopPlayerRemoteSetup : EntityEventListener<IPlayerState>
{
	private GameObject[] logs = new GameObject[0];

	[SerializeField]
	public Transform sledLookAt;

	[SerializeField]
	public Transform sledLookAtParent;

	[SerializeField]
	private StealItemTrigger RightHandStealTrigger;

	[SerializeField]
	public GameObject CarryBody;

	public itemConstrainToHand leftHand;

	public itemConstrainToHand rightHand;

	public itemConstrainToHand feet;

	private EnemyType lastHitEnemyType = EnemyType.none;

	public void setCurrentAttacker(enemyWeaponMelee attacker)
	{
		this.lastHitEnemyType = attacker.GetComponentInParent<enemyType>().Type;
	}

	public void hitFromEnemy(int damage)
	{
		PlayerHitByEnemey playerHitByEnemey = PlayerHitByEnemey.Create();
		playerHitByEnemey.Damage = damage;
		playerHitByEnemey.Target = this.entity;
		playerHitByEnemey.Send();
	}

	public void hitFromCreepy(int distance)
	{
		CreepHitPlayer creepHitPlayer = CreepHitPlayer.Create(this.entity.source);
		creepHitPlayer.damage = distance;
		creepHitPlayer.Send();
	}

	private void UpdateHands()
	{
		if (this.leftHand)
		{
			this.leftHand.Enable(base.state.itemInLeftHand, null);
		}
		if (this.rightHand)
		{
			this.rightHand.Enable(base.state.itemInRightHand, null);
		}
		if (this.feet)
		{
			this.feet.Enable(base.state.itemAtFeet, null);
		}
	}

	public override void Attached()
	{
		base.state.AddCallback("bodyHeld", delegate
		{
			this.CarryBody.SetActive(base.state.bodyHeld);
		});
		this.sledLookAt.parent = this.sledLookAtParent;
		this.sledLookAt.localPosition = new Vector3(0f, -0.41f, 0.912f);
		this.sledLookAt.localRotation = Quaternion.Euler(5f, 0f, 0f);
		base.InvokeRepeating("UpdateHands", 0.31f, 1f);
		if (this.leftHand)
		{
			base.state.AddCallback("itemInLeftHand", delegate
			{
				this.leftHand.Enable(base.state.itemInLeftHand, null);
			});
		}
		if (this.rightHand)
		{
			base.state.AddCallback("itemInRightHand", delegate
			{
				this.rightHand.Enable(base.state.itemInRightHand, this.RightHandStealTrigger);
			});
		}
		base.state.AddCallback("logHeld", delegate
		{
			if (base.state.logHeld)
			{
				this.logs[0].SetActive(true);
			}
			else
			{
				this.logs[0].SetActive(false);
				this.logs[1].SetActive(false);
			}
		});
		this.logs = new GameObject[]
		{
			base.transform.FindChildIncludingDeactivated("LogHeld1").gameObject,
			base.transform.FindChildIncludingDeactivated("LogHeld2").gameObject
		};
		base.state.AddCallback("logsHeld", delegate
		{
			if (base.state.logsHeld == 0)
			{
				this.logs[0].SetActive(false);
				this.logs[1].SetActive(false);
			}
			else if (base.state.logsHeld == 1)
			{
				this.logs[0].SetActive(true);
				this.logs[1].SetActive(false);
			}
			else if (base.state.logsHeld == 2)
			{
				this.logs[0].SetActive(true);
				this.logs[1].SetActive(true);
			}
		});
	}
}
