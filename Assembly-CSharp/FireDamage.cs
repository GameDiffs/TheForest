using Bolt;
using FMOD.Studio;
using PathologicalGames;
using System;
using UnityEngine;

public class FireDamage : EntityBehaviour
{
	public const int MaxBurnGenerations = 10;

	public bool DestroyWhenComplete;

	public GameObject DestroyTarget;

	public Material BurnMaterial;

	public FireDamageRendererMaterial[] AdditionalBurnMaterials;

	public bool Structure;

	public GameObject MyBurnt;

	public float FuelSeconds = 10f;

	[Range(0.5f, 5f)]
	public float SpreadInterval = 2f;

	protected float RandomizedSpreadInterval;

	public ParticleSystem FireParticleSystem;

	public FireDamagePoint[] FirePoints;

	public LayerMask SpreadLayers = -1;

	private int burnGeneration;

	public float MyFuel;

	[Header("FMOD")]
	public string burningEvent;

	private EventInstance burningEventInstance;

	private ParameterInstance sizeParameter;

	private ParameterInstance flareUpParameter;

	private float spreadTimer;

	public virtual bool isBurning
	{
		get;
		set;
	}

	protected virtual float usedFuelSeconds
	{
		get;
		set;
	}

	public override void Detached()
	{
		if (this.isBurning && this.MyBurnt)
		{
			UnityEngine.Object.Instantiate(this.MyBurnt, base.transform.position, base.transform.rotation);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Color color = Gizmos.color;
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.color = Color.red;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		for (int i = 0; i < this.FirePoints.Length; i++)
		{
			FireDamagePoint fireDamagePoint = this.FirePoints[i];
			if (fireDamagePoint.isBurning)
			{
				Gizmos.DrawSphere(fireDamagePoint.Position, fireDamagePoint.Radius);
			}
			else
			{
				Gizmos.DrawWireSphere(fireDamagePoint.Position, fireDamagePoint.Radius);
			}
		}
		Gizmos.matrix = matrix;
		Gizmos.color = color;
	}

	protected virtual void BurningOff()
	{
		this.isBurning = false;
	}

	private void OnEnable()
	{
		this.BurningOff();
		this.RandomizedSpreadInterval = this.SpreadInterval + UnityEngine.Random.value;
		for (int i = 0; i < this.FirePoints.Length; i++)
		{
			FireDamagePoint fireDamagePoint = this.FirePoints[i];
			fireDamagePoint.isBurning = false;
			fireDamagePoint.particleSystem = null;
		}
	}

	protected void StopBurning()
	{
		if (this.isBurning)
		{
			this.SetMaterialBurnAmount(0f);
		}
		this.BurningOff();
		if (!this.isBurning)
		{
			this.ResetFirePoints();
		}
		else
		{
			FireDamagePoint fireDamagePoint = this.FindClosestFirePoint(Vector3.zero);
			if (fireDamagePoint != null)
			{
				this.BurnPoint(fireDamagePoint);
			}
		}
	}

	protected void ResetFirePoints()
	{
		for (int i = 0; i < this.FirePoints.Length; i++)
		{
			FireDamagePoint fireDamagePoint = this.FirePoints[i];
			if (fireDamagePoint != null)
			{
				fireDamagePoint.isBurning = false;
				if (fireDamagePoint.particleSystem)
				{
					PoolManager.Pools["Particles"].Despawn(fireDamagePoint.particleSystem.transform);
					fireDamagePoint.particleSystem = null;
				}
			}
		}
	}

	private void OnDestroy()
	{
		this.StopBurningEvent();
		this.StopBurning();
	}

	private void OnDisable()
	{
		this.StopBurningEvent();
		this.StopBurning();
	}

	private void Burn()
	{
		if (this.MyFuel == 0f)
		{
			this.MyFuel = 15f;
		}
		this.Ignite(base.transform.position, 0);
	}

	public void Ignite(Vector3 worldBurnPosition, int generation = 0)
	{
		if (this.isBurning)
		{
			return;
		}
		this.burnGeneration = generation;
		this.spreadTimer = 0f;
		this.isBurning = true;
		FireDamagePoint fireDamagePoint = this.FindClosestFirePoint(base.transform.InverseTransformPoint(worldBurnPosition));
		if (fireDamagePoint != null)
		{
			this.BurnPoint(fireDamagePoint);
			this.UpdateSizeParameter();
		}
		if (this.BurnMaterial && !this.Structure)
		{
			base.GetComponent<Renderer>().material = this.BurnMaterial;
		}
		for (int i = 0; i < this.AdditionalBurnMaterials.Length; i++)
		{
			this.AdditionalBurnMaterials[i].renderer.material = this.AdditionalBurnMaterials[i].material;
		}
	}

	private void BurnPoint(FireDamagePoint firePoint)
	{
		ParticleSystem particleSystem = PoolManager.Pools["Particles"].Spawn(this.FireParticleSystem, base.transform.TransformPoint(firePoint.Position), Quaternion.identity);
		if (particleSystem != null)
		{
			firePoint.isBurning = true;
			firePoint.particleSystem = particleSystem.GetComponent<FireParticle>();
			firePoint.particleSystem.MyFuel = this.MyFuel;
			firePoint.particleSystem.Parent = this;
			if (this.flareUpParameter != null)
			{
				UnityUtil.ERRCHECK(this.flareUpParameter.setValue(1f));
				UnityUtil.ERRCHECK(this.flareUpParameter.setValue(0f));
			}
		}
	}

	public void ExtinguishPoint(FireParticle particle)
	{
		for (int i = 0; i < this.FirePoints.Length; i++)
		{
			FireDamagePoint fireDamagePoint = this.FirePoints[i];
			if (fireDamagePoint.particleSystem == particle)
			{
				fireDamagePoint.particleSystem = null;
				return;
			}
		}
	}

	private FireDamagePoint FindClosestFirePoint(Vector3 localPosition)
	{
		FireDamagePoint result = null;
		float num = 999999f;
		for (int i = 0; i < this.FirePoints.Length; i++)
		{
			FireDamagePoint fireDamagePoint = this.FirePoints[i];
			if (!fireDamagePoint.isBurning)
			{
				float sqrMagnitude = (fireDamagePoint.Position - localPosition).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = fireDamagePoint;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	private void Spread()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		for (int i = 0; i < this.FirePoints.Length; i++)
		{
			FireDamagePoint fireDamagePoint = this.FirePoints[i];
			if (fireDamagePoint.isBurning)
			{
				FireDamagePoint fireDamagePoint2 = this.FindClosestFirePoint(fireDamagePoint.Position);
				if (fireDamagePoint2 != null)
				{
					this.BurnPoint(fireDamagePoint2);
				}
			}
		}
		if (this.burnGeneration + 1 < 10 && this.SpreadLayers != 0)
		{
			for (int j = 0; j < this.FirePoints.Length; j++)
			{
				FireDamagePoint fireDamagePoint3 = this.FirePoints[j];
				if (fireDamagePoint3.isBurning)
				{
					Vector3 vector = base.transform.TransformPoint(fireDamagePoint3.Position);
					Collider[] array = Physics.OverlapSphere(vector, fireDamagePoint3.Radius, this.SpreadLayers);
					for (int k = 0; k < array.Length; k++)
					{
						Collider collider = array[k];
						if (!collider.transform.IsChildOf(base.transform))
						{
							FireDamage componentInChildren = collider.GetComponentInChildren<FireDamage>();
							if (componentInChildren)
							{
								componentInChildren.Ignite(vector, this.burnGeneration + 1);
							}
							else
							{
								collider.SendMessage("Burn", SendMessageOptions.DontRequireReceiver);
							}
						}
					}
				}
			}
		}
	}

	private void UpdateSizeParameter()
	{
		if (this.sizeParameter != null && this.MyFuel != 0f)
		{
			float num = 0f;
			for (int i = 0; i < this.FirePoints.Length; i++)
			{
				FireDamagePoint fireDamagePoint = this.FirePoints[i];
				if (fireDamagePoint.particleSystem != null)
				{
					num += fireDamagePoint.particleSystem.MyFuel;
				}
			}
			UnityUtil.ERRCHECK(this.sizeParameter.setValue(100f * num / (this.MyFuel * (float)this.FirePoints.Length)));
		}
	}

	private void StartBurningEvent()
	{
		if (this.burningEventInstance == null && !string.IsNullOrEmpty(this.burningEvent))
		{
			this.burningEventInstance = FMODCommon.PlayOneshot(this.burningEvent, base.transform);
			if (this.burningEventInstance != null)
			{
				this.burningEventInstance.getParameter("size", out this.sizeParameter);
				this.burningEventInstance.getParameter("flare up", out this.flareUpParameter);
			}
		}
	}

	private void StopBurningEvent()
	{
		if (this.burningEventInstance != null)
		{
			UnityUtil.ERRCHECK(this.burningEventInstance.stop(STOP_MODE.ALLOWFADEOUT));
			this.burningEventInstance = null;
			this.sizeParameter = null;
			this.flareUpParameter = null;
		}
	}

	private void Update()
	{
		if (this.isBurning)
		{
			float num = Mathf.Clamp01(this.usedFuelSeconds / this.FuelSeconds);
			this.usedFuelSeconds += Time.deltaTime;
			if (num >= 0.99f)
			{
				this.StopBurningEvent();
				this.FinishedBurning();
			}
			else
			{
				this.SetMaterialBurnAmount(num);
				this.spreadTimer += Time.deltaTime;
				if (this.spreadTimer > this.RandomizedSpreadInterval)
				{
					this.spreadTimer -= this.RandomizedSpreadInterval;
					this.Spread();
				}
				this.StartBurningEvent();
				this.UpdateSizeParameter();
			}
		}
	}

	protected void SetMaterialBurnAmount(float burnAmount)
	{
		if (this.BurnMaterial && !this.Structure)
		{
			base.GetComponent<Renderer>().material.SetFloat("_BurnAmount", burnAmount);
		}
		for (int i = 0; i < this.AdditionalBurnMaterials.Length; i++)
		{
			this.AdditionalBurnMaterials[i].renderer.material.SetFloat("_BurnAmount", burnAmount);
		}
	}

	protected virtual void FinishedBurning()
	{
		if (this.MyBurnt && !BoltNetwork.isClient)
		{
			GameObject value = (GameObject)UnityEngine.Object.Instantiate(this.MyBurnt, base.transform.position, base.transform.rotation);
			base.SendMessage("Burnt", value, SendMessageOptions.DontRequireReceiver);
		}
		if (this.DestroyWhenComplete)
		{
			if (BoltNetwork.isServer && this.entity && this.entity.isAttached)
			{
				BoltNetwork.Destroy((!this.DestroyTarget) ? base.gameObject : this.DestroyTarget);
			}
			else
			{
				UnityEngine.Object.Destroy((!this.DestroyTarget) ? base.gameObject : this.DestroyTarget);
			}
		}
		else
		{
			this.StopBurning();
			this.usedFuelSeconds = 0f;
		}
	}
}
