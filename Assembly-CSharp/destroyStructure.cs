using Bolt;
using System;
using UnityEngine;

public class destroyStructure : EntityBehaviour
{
	public GameObject dynamicWall;

	public int health = 300;

	public GameObject destroyGo;

	public GameObject dust;

	private bool IsHit;

	private void Hit(int damage)
	{
		if (!this.IsHit)
		{
			this.IsHit = true;
			this.health -= damage;
			if (this.health > 0)
			{
				this.IsHit = false;
				return;
			}
			if (BoltNetwork.isRunning && this.entity)
			{
				DestroyBuilding destroyBuilding = DestroyBuilding.Raise(GlobalTargets.OnlyServer);
				destroyBuilding.BuildingEntity = this.entity;
				destroyBuilding.Send();
				UnityEngine.Object.Instantiate(Resources.Load("WallBuiltDynamic_MP", typeof(GameObject)), base.transform.position, base.transform.rotation);
				return;
			}
			UnityEngine.Object.Instantiate(this.dynamicWall, base.transform.position, base.transform.rotation);
			if (this.dust)
			{
				UnityEngine.Object.Instantiate(this.dust, base.transform.position, base.transform.rotation);
			}
			if (this.destroyGo != null)
			{
				UnityEngine.Object.Destroy(this.destroyGo);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
