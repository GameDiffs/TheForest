using PathologicalGames;
using System;
using TheForest.Utils;
using UnityEngine;

public abstract class LOD_Base : MonoBehaviour
{
	public Transform High;

	public Transform Mid;

	public Transform Low;

	public GameObject billboard;

	private bool isSpawned;

	private bool canForceWorkScheduler = true;

	[HideInInspector]
	public bool DontSpawn;

	public Transform CurrentLodTransform;

	protected CustomBillboard cb;

	protected int currentLOD = -1;

	protected int BillboardId = -1;

	private TheForestQualitySettings.DrawDistances maxDrawDistanceSetting;

	private LOD_Stats stats = new LOD_Stats();

	protected Vector3 _position;

	private int wsToken;

	private bool lodWasDestroyed;

	public abstract LOD_Settings LodSettings
	{
		get;
	}

	public abstract SpawnPool Pool
	{
		get;
	}

	public virtual bool DestroyInsteadOfDisable
	{
		get
		{
			return false;
		}
	}

	public virtual bool UseLow
	{
		get
		{
			return true;
		}
	}

	public int CurrentLOD
	{
		get
		{
			return this.currentLOD;
		}
	}

	private void Awake()
	{
		this.maxDrawDistanceSetting = this.LodSettings.GetNewObjectMaxDrawDistance;
	}

	protected virtual void OnEnable()
	{
		this.CurrentLodTransform = null;
		this._position = base.transform.position;
		if (this.cb == null)
		{
			if (this.billboard != null)
			{
				this.cb = this.billboard.GetComponent<CustomBillboard>();
			}
			if (this.BillboardId == -1 && this.cb != null)
			{
				this.BillboardId = this.cb.Register(this._position, base.transform.eulerAngles.y);
			}
		}
		else
		{
			this.cb.SetAlive(this.BillboardId, true);
		}
		this.wsToken = WorkScheduler.Register(new WorkScheduler.Task(this.RefreshLODs), this._position, this.canForceWorkScheduler);
		this.canForceWorkScheduler = false;
	}

	protected virtual void OnDisable()
	{
		WorkScheduler.Unregister(new WorkScheduler.Task(this.RefreshLODs), this.wsToken);
		this.canForceWorkScheduler = true;
		if (this.lodWasDestroyed && this.Pool)
		{
			this.Pool.KillInstance(this.CurrentLodTransform);
		}
		else
		{
			this.DespawnCurrent();
		}
		this.lodWasDestroyed = false;
		this.CurrentLodTransform = null;
		this.isSpawned = false;
		if (this.BillboardId >= 0 && this.cb != null)
		{
			this.cb.SetAlive(this.BillboardId, false);
		}
		if (!base.gameObject.activeSelf)
		{
			base.enabled = true;
		}
	}

	public void DespawnCurrent()
	{
		if (this.CurrentLodTransform && this.CurrentLodTransform.gameObject.activeSelf && !this.stats.StopPooling && Scene.ActiveMB)
		{
			LOD_Stats.Current = this.stats;
			if (this.Pool)
			{
				this.Pool.Despawn(this.CurrentLodTransform);
			}
			else
			{
				UnityEngine.Object.Destroy(this.CurrentLodTransform.gameObject);
			}
			LOD_Stats.Current = null;
			this.CurrentLodTransform = null;
			this.isSpawned = false;
		}
	}

	private int GetLOD()
	{
		return this.LodSettings.GetLOD(this._position, this.currentLOD);
	}

	public virtual void SetLOD(int lod)
	{
		if (this.stats.StopPooling)
		{
			return;
		}
		this.DespawnCurrent();
		if (this.DontSpawn)
		{
			return;
		}
		Transform transform = null;
		switch (lod)
		{
		case 0:
			transform = this.High;
			break;
		case 1:
			transform = this.Mid;
			break;
		case 2:
			if (this.UseLow)
			{
				transform = this.Low;
			}
			break;
		}
		if (transform != null)
		{
			LOD_Stats.Current = this.stats;
			if (this.Pool)
			{
				this.CurrentLodTransform = this.Pool.Spawn(transform, this._position, base.transform.rotation);
			}
			else
			{
				this.CurrentLodTransform = (UnityEngine.Object.Instantiate(transform, this._position, base.transform.rotation) as Transform);
			}
			LOD_Stats.Current = null;
			if (this.CurrentLodTransform)
			{
				this.CurrentLodTransform.SendMessage("SetLodBase", this, SendMessageOptions.DontRequireReceiver);
				this.isSpawned = true;
			}
		}
		this.currentLOD = lod;
		base.SendMessage("LodChanged", this.currentLOD, SendMessageOptions.DontRequireReceiver);
	}

	private void RefreshLODs()
	{
		if (this.DontSpawn || this.maxDrawDistanceSetting < TheForestQualitySettings.UserSettings.DrawDistance)
		{
			this.DespawnCurrent();
			if (this.currentLOD != -2)
			{
				if (this.BillboardId >= 0 && this.cb != null)
				{
					this.cb.SetAlive(this.BillboardId, false);
				}
				this.currentLOD = -2;
			}
			return;
		}
		if (this.currentLOD == -2 && this.BillboardId >= 0 && this.cb != null)
		{
			this.cb.SetAlive(this.BillboardId, true);
		}
		bool flag = this.CurrentLodTransform && !this.CurrentLodTransform.gameObject.activeSelf;
		if (this.isSpawned && (!this.CurrentLodTransform || flag))
		{
			this.CurrentLodTransform = null;
			this.lodWasDestroyed = !flag;
			base.enabled = false;
			if (this.DestroyInsteadOfDisable)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			return;
		}
		int lOD = this.GetLOD();
		if (lOD != this.currentLOD)
		{
			this.SetLOD(lOD);
		}
	}

	public void ForceLOD()
	{
	}

	public bool Burn()
	{
		bool result = false;
		if (this.CurrentLodTransform)
		{
			FirePoint[] componentsInChildren = this.CurrentLodTransform.GetComponentsInChildren<FirePoint>();
			FirePoint[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				FirePoint firePoint = array[i];
				firePoint.startFire();
				result = true;
			}
		}
		return result;
	}
}
