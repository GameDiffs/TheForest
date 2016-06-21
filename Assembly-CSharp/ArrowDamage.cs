using Bolt;
using System;
using TheForest.Utils;
using UnityEngine;

public class ArrowDamage : MonoBehaviour
{
	private arrowTrajectory at;

	public LayerMask layers;

	public GameObject MyPickUp;

	public GameObject parent;

	public Rigidbody PhysicBody;

	public bool spearType;

	public float damage;

	[Header("FMOD")]
	public string hitTreeEvent;

	public string hitStructureEvent;

	public string hitGroundEvent;

	private CapsuleCollider bodyCollider;

	private RaycastHit hit;

	private bool Live = true;

	private bool ignoreTerrain;

	private void Start()
	{
		if (this.spearType)
		{
			this.bodyCollider = base.transform.root.GetComponent<CapsuleCollider>();
		}
		this.at = base.transform.root.GetComponent<arrowTrajectory>();
	}

	private void OnEnable()
	{
		this.Live = true;
		this.ignoreTerrain = false;
	}

	private void LateUpdate()
	{
		if (this.Live && this.PhysicBody)
		{
			float num = this.PhysicBody.velocity.magnitude * 1.25f * Time.deltaTime;
			Vector3 vector = base.transform.position + base.transform.forward * -num;
			if (num > 0f && Physics.Raycast(base.transform.position, base.transform.forward, out this.hit, num, this.layers))
			{
				if (this.hit.transform.CompareTag("Tree") || this.hit.transform.CompareTag("Target") || this.hit.transform.gameObject.layer == 20)
				{
					if (this.spearType)
					{
						if (this.bodyCollider)
						{
							this.bodyCollider.isTrigger = true;
						}
						this.PhysicBody.transform.position = this.hit.point - base.transform.forward * 2.1f;
					}
					else
					{
						this.PhysicBody.transform.position = this.hit.point - base.transform.forward * 0.35f;
					}
					this.PhysicBody.velocity = Vector3.zero;
					this.PhysicBody.isKinematic = true;
					this.MyPickUp.SetActive(true);
					this.Live = false;
					FMODCommon.PlayOneshotNetworked(this.hitTreeEvent, base.transform, FMODCommon.NetworkRole.Any);
				}
				else if (this.hit.transform.CompareTag("enemyCollide") || this.hit.transform.tag == "lb_bird" || this.hit.transform.CompareTag("animalCollide") || this.hit.transform.CompareTag("Fish"))
				{
					int num2;
					if (this.spearType)
					{
						num2 = Mathf.FloorToInt(40f);
					}
					else
					{
						num2 = Mathf.FloorToInt(2f * (this.PhysicBody.velocity.magnitude / 7f));
						if (num2 > 22)
						{
							num2 = 22;
						}
					}
					if (this.spearType)
					{
						this.PhysicBody.velocity = Vector3.zero;
						this.PhysicBody.isKinematic = false;
						this.PhysicBody.useGravity = true;
						this.Live = false;
						this.MyPickUp.SetActive(true);
					}
					if (BoltNetwork.isClient)
					{
						PlayerHitEnemy playerHitEnemy = PlayerHitEnemy.Raise(GlobalTargets.OnlyServer);
						playerHitEnemy.Target = this.hit.transform.GetComponentInParent<BoltEntity>();
						playerHitEnemy.getAttackDirection = 3;
						playerHitEnemy.getAttackerType = 4;
						playerHitEnemy.Hit = num2;
						playerHitEnemy.Send();
					}
					else if (this.hit.transform.CompareTag("enemyRoot"))
					{
						this.hit.transform.gameObject.SendMessage("getAttackDirection", 3, SendMessageOptions.DontRequireReceiver);
						this.hit.transform.gameObject.SendMessage("getAttackerType", 4, SendMessageOptions.DontRequireReceiver);
						GameObject closestPlayerFromPos = Scene.SceneTracker.GetClosestPlayerFromPos(base.transform.position);
						this.hit.transform.gameObject.SendMessage("getAttacker", closestPlayerFromPos, SendMessageOptions.DontRequireReceiver);
						this.hit.transform.gameObject.SendMessage("Hit", num2, SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						this.hit.transform.gameObject.SendMessageUpwards("getAttackDirection", 3, SendMessageOptions.DontRequireReceiver);
						this.hit.transform.gameObject.SendMessageUpwards("getAttackerType", 4, SendMessageOptions.DontRequireReceiver);
						GameObject closestPlayerFromPos2 = Scene.SceneTracker.GetClosestPlayerFromPos(base.transform.position);
						this.hit.transform.gameObject.SendMessageUpwards("getAttacker", closestPlayerFromPos2, SendMessageOptions.DontRequireReceiver);
						this.hit.transform.gameObject.SendMessageUpwards("Hit", num2, SendMessageOptions.DontRequireReceiver);
					}
					this.MyPickUp.SetActive(true);
					this.Live = false;
				}
				else if (this.hit.transform.CompareTag("PlayerNet"))
				{
					if (BoltNetwork.isRunning)
					{
						BoltEntity componentInParent = this.hit.transform.GetComponentInParent<BoltEntity>();
						if (componentInParent)
						{
							HitPlayer.Create(componentInParent, EntityTargets.OnlyOwner).Send();
						}
					}
				}
				else if (this.hit.transform.CompareTag("TerrainMain") || this.hit.transform.CompareTag("structure") || this.hit.transform.CompareTag("SLTier1") || this.hit.transform.CompareTag("SLTier2") || this.hit.transform.CompareTag("SLTier3"))
				{
					if (this.ignoreTerrain && this.hit.transform.CompareTag("TerrainMain"))
					{
						this.ignoreTerrain = false;
						Physics.IgnoreCollision(base.GetComponent<Collider>(), Terrain.activeTerrain.GetComponent<Collider>(), false);
					}
					else
					{
						if (this.spearType)
						{
							if (this.bodyCollider)
							{
								this.bodyCollider.isTrigger = true;
							}
							this.PhysicBody.transform.position = this.hit.point - base.transform.forward * 2.1f;
						}
						else
						{
							this.PhysicBody.transform.position = this.hit.point - base.transform.forward * 0.35f;
						}
						this.PhysicBody.velocity = Vector3.zero;
						this.PhysicBody.isKinematic = true;
						this.MyPickUp.SetActive(true);
						if (this.at)
						{
							this.at.enabled = false;
						}
						this.Live = false;
						if (this.hit.transform.CompareTag("TerrainMain"))
						{
							FMODCommon.PlayOneshotNetworked(this.hitGroundEvent, base.transform, FMODCommon.NetworkRole.Any);
						}
						else
						{
							FMODCommon.PlayOneshotNetworked(this.hitStructureEvent, base.transform, FMODCommon.NetworkRole.Any);
						}
					}
				}
				else if (this.hit.transform.CompareTag("CaveDoor"))
				{
					this.ignoreTerrain = true;
					Physics.IgnoreCollision(base.GetComponent<Collider>(), Terrain.activeTerrain.GetComponent<Collider>(), true);
				}
				else if (!this.hit.collider.isTrigger && !this.hit.transform.Equals(base.transform.parent) && this.hit.transform.CompareTag("enemyRoot"))
				{
					this.PhysicBody.velocity /= 4f;
					this.MyPickUp.SetActive(true);
					this.Live = false;
				}
				else if (!this.hit.collider.isTrigger && !this.hit.transform.Equals(base.transform.parent))
				{
					this.PhysicBody.velocity /= 2f;
					this.MyPickUp.SetActive(true);
					this.Live = false;
					if (this.at)
					{
						this.at.enabled = false;
					}
				}
				if (!this.Live)
				{
					this.parent.BroadcastMessage("OnArrowHit", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	private void disableLive()
	{
		this.Live = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.PhysicBody && other.gameObject.CompareTag("SmallTree"))
		{
			other.gameObject.SendMessage("Hit", 2f * this.PhysicBody.velocity.magnitude, SendMessageOptions.DontRequireReceiver);
		}
		base.Invoke("destroyMe", 90f);
	}

	private void destroyMe()
	{
		if (!BoltNetwork.isRunning || !base.GetComponentInParent<BoltEntity>().isAttached)
		{
			UnityEngine.Object.Destroy(this.parent);
		}
		else if (BoltNetwork.isServer)
		{
			BoltNetwork.Destroy(this.parent.gameObject);
		}
	}
}
