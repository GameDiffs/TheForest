using System;
using UnityEngine;

public class clsragdollify : MonoBehaviour
{
	public Transform vargamragdoll;

	public bool doCreepySkin;

	public bool animal;

	public bool fish;

	public bool bird;

	public bool burning;

	public bool alreadyBurnt;

	public bool spinRagdoll;

	public bool hackVelocity;

	public Transform hitTr;

	public Vector3 getVelocity;

	private MaterialPropertyBlock bloodPropertyBlock;

	private Fish fishScript;

	private EnemyHealth health;

	private void Awake()
	{
		this.bloodPropertyBlock = new MaterialPropertyBlock();
		if (this.fish)
		{
			this.fishScript = base.transform.GetComponent<Fish>();
		}
		if (this.doCreepySkin)
		{
			this.health = base.transform.GetComponent<EnemyHealth>();
		}
	}

	private void metcopytransforms(Transform varpsource, Transform varpdestination, Vector3 varpvelocity = default(Vector3))
	{
		varpdestination.position = varpsource.position;
		varpdestination.rotation = varpsource.rotation;
		if (varpvelocity != Vector3.zero)
		{
			Rigidbody component = varpdestination.GetComponent<Rigidbody>();
			if (component != null && this.hitTr != null)
			{
				component.velocity = varpvelocity * 1.5f + this.hitTr.right * UnityEngine.Random.Range(-5f, 5f);
				if (component.GetComponent<ConstantForce>() && this.spinRagdoll)
				{
					component.GetComponent<ConstantForce>().torque = Vector3.up * 1E+08f + base.transform.right * 1000000f;
					this.spinRagdoll = false;
				}
			}
		}
		if (!this.hackVelocity)
		{
			foreach (Transform transform in varpdestination)
			{
				Transform transform2 = varpsource.Find(transform.name);
				if (transform2)
				{
					this.metcopytransforms(transform2, transform, varpvelocity * 1f);
				}
			}
		}
	}

	public Transform metgoragdoll(Vector3 varpvelocity = default(Vector3))
	{
		Transform transform = UnityEngine.Object.Instantiate(this.vargamragdoll, base.transform.position, base.transform.rotation) as Transform;
		transform.localScale = base.transform.localScale;
		this.metcopytransforms(base.transform, transform, varpvelocity * 1f);
		if (this.doCreepySkin)
		{
			transform.gameObject.SendMessage("setSkin", this.health.MySkin.sharedMaterial, SendMessageOptions.DontRequireReceiver);
			this.health.MySkin.GetPropertyBlock(this.bloodPropertyBlock);
			transform.gameObject.SendMessage("setSkinDamageProperty", this.bloodPropertyBlock, SendMessageOptions.DontRequireReceiver);
			if (BoltNetwork.isServer)
			{
				this.setSkinDamageMP(transform.gameObject);
			}
		}
		if (this.animal)
		{
			animalSpawnFunctions component = base.transform.root.GetComponent<animalSpawnFunctions>();
			if (component)
			{
				transform.gameObject.SendMessage("setSkin", component.meshRenderer.sharedMaterial, SendMessageOptions.DontRequireReceiver);
			}
		}
		if (this.bird)
		{
			lb_Bird component2 = base.transform.GetComponent<lb_Bird>();
			transform.gameObject.SendMessage("setSkin", component2.skin.sharedMaterial, SendMessageOptions.DontRequireReceiver);
		}
		if (this.fish)
		{
			transform.gameObject.SendMessage("doSkinSetup", this.fishScript.fishTypeInt, SendMessageOptions.DontRequireReceiver);
		}
		if (this.burning)
		{
			transform.gameObject.SendMessage("enableFire", SendMessageOptions.DontRequireReceiver);
		}
		if (this.alreadyBurnt)
		{
			transform.gameObject.SendMessage("enableBurntSkin", SendMessageOptions.DontRequireReceiver);
		}
		this.burning = false;
		this.alreadyBurnt = false;
		return transform;
	}

	private void setSkinDamageMP(GameObject ragdoll)
	{
		if (BoltNetwork.isServer)
		{
			IMutantState state = base.transform.parent.GetComponent<BoltEntity>().GetState<IMutantState>();
			BoltNetwork.Attach(ragdoll, new CoopMutantDummyToken
			{
				Scale = base.transform.localScale,
				skinDamage1 = this.bloodPropertyBlock.GetFloat("_Damage1"),
				skinDamage2 = this.bloodPropertyBlock.GetFloat("_Damage2"),
				skinDamage3 = this.bloodPropertyBlock.GetFloat("_Damage3"),
				skinDamage4 = this.bloodPropertyBlock.GetFloat("_Damage4")
			});
		}
	}
}
