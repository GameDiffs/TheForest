using System;
using TheForest.Utils;
using TheForest.World;
using UnityEngine;

public class ExplodeTreeStump : MonoBehaviour
{
	public GameObject _idleIfPresent;

	public GameObject _blownUpStump;

	private float _hp = 250f;

	private void OnSpawned()
	{
		this._hp = 250f;
	}

	private void lookAtExplosion(Vector3 position)
	{
		Vector3 b = new Vector3(1f, 0f, 1f);
		if (!this._idleIfPresent && !BoltNetwork.isClient && Vector3.Distance(Vector3.Scale(position, b), Vector3.Scale(base.transform.position, b)) < 3.5f)
		{
			LocalPlayer.Sfx.PlayBreakWood(base.gameObject);
			LOD_Stump componentInParent = base.GetComponentInParent<LOD_Stump>();
			if (componentInParent)
			{
				LOD_Trees component = componentInParent.transform.parent.GetComponent<LOD_Trees>();
				if (component.Pool.IsSpawned(base.transform))
				{
					base.transform.parent = component.Pool.transform;
					component.Pool.Despawn(base.transform);
				}
				if (BoltNetwork.isRunning)
				{
					CoopTreeId component2 = component.GetComponent<CoopTreeId>();
					if (component2)
					{
						component2.Goto_Removed();
					}
				}
				this.Finalize(component, componentInParent.gameObject);
			}
			else
			{
				TreeHealth componentInParent2 = base.GetComponentInParent<TreeHealth>();
				if (componentInParent2 && componentInParent2.LodTree)
				{
					if (BoltNetwork.isRunning)
					{
						CoopTreeId component3 = componentInParent2.LodTree.GetComponent<CoopTreeId>();
						if (component3)
						{
							component3.Goto_Removed();
						}
					}
					this.Finalize(componentInParent2.LodTree, componentInParent2.gameObject);
				}
			}
		}
	}

	private void LocalizedHit(LocalizedHitData data)
	{
		if (!this._idleIfPresent)
		{
			Prefabs.Instance.SpawnWoodHitPS(data._position, Quaternion.LookRotation(base.transform.position - data._position));
			if (this._hp > 0f)
			{
				this._hp -= data._damage;
				if (this._hp <= 0f)
				{
					this.lookAtExplosion(base.transform.position);
				}
			}
		}
	}

	private void Finalize(LOD_Trees lt, GameObject go)
	{
		if (this._blownUpStump)
		{
			if (!BoltNetwork.isRunning)
			{
				this.navCheck();
				UnityEngine.Object.Instantiate(this._blownUpStump, base.transform.position, base.transform.rotation);
			}
			else if (BoltNetwork.isServer)
			{
				this.navCheck();
				BoltNetwork.Instantiate(this._blownUpStump, base.transform.position, base.transform.rotation);
			}
		}
		UnityEngine.Object.Destroy(lt);
		UnityEngine.Object.Destroy(go);
	}

	private void navCheck()
	{
		Collider component = base.transform.GetComponent<Collider>();
		if (component)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate((GameObject)Resources.Load("dummyNavRemove"), base.transform.position, base.transform.rotation);
			gameObject.SendMessage("doDummyNavRemove", component.bounds, SendMessageOptions.DontRequireReceiver);
		}
	}
}
