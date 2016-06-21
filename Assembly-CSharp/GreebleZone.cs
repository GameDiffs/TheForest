using System;
using TheForest.Utils;
using UnityEngine;

public class GreebleZone : MonoBehaviour
{
	public struct GreebleInstanceData
	{
		public bool Spawned;

		public bool Destroyed;

		public float CreationTime;
	}

	[Range(1f, 1000f)]
	public int MinInstances = 10;

	[Range(1f, 1000f)]
	public int MaxInstances = 10;

	public GreebleShape Shape;

	public GreebleRayDirection Direction;

	[Range(1f, 50f)]
	public float Radius = 10f;

	public Vector3 Size = new Vector3(50f, 50f, 50f);

	[Range(10f, 500f)]
	public float ToggleDistance = 50f;

	[Range(0f, 100000f)]
	public int RandomSeed;

	public GreebleSelection RandomSelection;

	public bool AllowRegrowth;

	public bool MpSync;

	public GreebleDefinition[] GreebleDefinitions;

	public Action<int, GameObject> OnSpawned;

	private int InstanceCount;

	private GreebleZone.GreebleInstanceData[] InstanceData;

	private bool currentlyVisible;

	private GameObject[] instances;

	private int wsToken = -1;

	private int scheduledSpawnIndex;

	public GreebleZonesManager.GZData Data
	{
		get;
		set;
	}

	public bool CurrentlyVisible
	{
		get
		{
			return this.currentlyVisible;
		}
	}

	public GameObject[] Instances
	{
		get
		{
			return this.instances;
		}
	}

	private void Awake()
	{
		if (this.MpSync && BoltNetwork.isClient)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			base.enabled = false;
		}
	}

	private void Start()
	{
		switch (this.RandomSelection)
		{
		case GreebleSelection.RandomizeChances:
		{
			GreebleDefinition[] greebleDefinitions = this.GreebleDefinitions;
			for (int i = 0; i < greebleDefinitions.Length; i++)
			{
				GreebleDefinition greebleDefinition = greebleDefinitions[i];
				greebleDefinition.Chance = GreebleUtility.ProceduralValue(1, 100);
			}
			break;
		}
		case GreebleSelection.SelectOne:
		{
			int num = GreebleUtility.ProceduralValue(0, this.GreebleDefinitions.Length);
			for (int j = 0; j < this.GreebleDefinitions.Length; j++)
			{
				this.GreebleDefinitions[j].Chance = ((j != num) ? 0 : 1);
			}
			break;
		}
		}
	}

	private void OnEnable()
	{
		if (this.Data == null)
		{
			this.Data = new GreebleZonesManager.GZData();
			this.Data._seed = -1;
			this.Data._instancesState = new byte[this.MaxInstances];
			for (int i = 0; i < this.Data._instancesState.Length; i++)
			{
				this.Data._instancesState[i] = 254;
			}
		}
		this.Data.GZ = this;
		this.wsToken = WorkScheduler.Register(new WorkScheduler.Task(this.UpdateVisibility), base.transform.position, false);
	}

	private void OnDisable()
	{
		if (this.wsToken >= 0)
		{
			WorkScheduler.Unregister(new WorkScheduler.Task(this.UpdateVisibility), this.wsToken);
			this.Despawn();
		}
	}

	private void UpdateVisibility()
	{
		float num;
		if (!this.MpSync)
		{
			num = (GreeblePlugin.GetCameraPosition() - base.transform.position).magnitude;
		}
		else
		{
			num = Scene.SceneTracker.GetClosestPlayerDistanceFromPos(base.transform.position);
		}
		float num2 = 0f;
		GreebleShape shape = this.Shape;
		if (shape != GreebleShape.Sphere)
		{
			if (shape == GreebleShape.Box)
			{
				Vector2 vector = new Vector2(this.Size.x, this.Size.z);
				num2 = vector.magnitude;
			}
		}
		else
		{
			num2 = this.Radius;
		}
		float num3 = num - num2;
		bool flag = num3 < this.ToggleDistance * TheForestQualitySettings.UserSettings.DrawDistanceGreebleRatio;
		if (flag != this.currentlyVisible)
		{
			if (flag)
			{
				this.Spawn();
			}
			else
			{
				this.Despawn();
			}
		}
	}

	public void Refresh()
	{
		this.Despawn();
		this.Spawn();
	}

	private int GetRandomSeed()
	{
		int num;
		if (this.Data == null || this.Data._seed == -1)
		{
			num = UnityEngine.Random.Range(0, 8675309);
			Vector3 position = base.transform.position;
			num = (int)position.x + (int)position.y + (int)position.z + this.RandomSeed;
			if (this.Data != null)
			{
				this.Data._seed = num;
			}
		}
		else
		{
			num = this.Data._seed;
		}
		return num;
	}

	private void Spawn()
	{
		this.Despawn();
		UnityEngine.Random.seed = this.GetRandomSeed();
		this.InstanceCount = GreebleUtility.ProceduralValue(this.MinInstances, this.MaxInstances + 1);
		if (this.InstanceData == null || this.InstanceData.Length != this.InstanceCount)
		{
			this.InstanceData = new GreebleZone.GreebleInstanceData[this.InstanceCount];
			for (int i = 0; i < this.InstanceData.Length; i++)
			{
				this.InstanceData[i].Spawned = false;
				if (this.Data != null && Application.isPlaying)
				{
					this.InstanceData[i].Destroyed = (this.Data._instancesState[i] == 255);
				}
				else
				{
					this.InstanceData[i].Destroyed = false;
				}
				this.InstanceData[i].CreationTime = 0f;
			}
		}
		if (this.instances == null || this.instances.Length != this.InstanceCount)
		{
			this.instances = new GameObject[this.InstanceCount];
		}
		this.currentlyVisible = true;
		this.scheduledSpawnIndex = 0;
		this.ScheduledSpawn();
		UnityEngine.Random.seed = this.GetRandomSeed();
	}

	private void ScheduledSpawn()
	{
		if (!this.currentlyVisible)
		{
			return;
		}
		if (this.scheduledSpawnIndex < this.InstanceCount)
		{
			this.SpawnIndex(this.scheduledSpawnIndex);
		}
		this.scheduledSpawnIndex++;
		if (this.scheduledSpawnIndex < this.InstanceCount)
		{
			WorkScheduler.RegisterOneShot(new WorkScheduler.Task(this.ScheduledSpawn));
		}
	}

	private void SpawnIndex(int index)
	{
		if (this.InstanceData[index].Destroyed)
		{
			return;
		}
		UnityEngine.Random.seed = this.GetRandomSeed() + index;
		GreebleDefinition greebleDefinition = GreebleUtility.ProceduralGreebleType(this.GreebleDefinitions, (Time.timeSinceLevelLoad - this.InstanceData[index].CreationTime) / 60f);
		if (greebleDefinition == null)
		{
			UnityEngine.Random.seed = this.GetRandomSeed();
			return;
		}
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.down;
		float distance = this.Radius;
		GreebleShape shape = this.Shape;
		if (shape != GreebleShape.Sphere)
		{
			if (shape == GreebleShape.Box)
			{
				Vector3 vector3 = this.Size * 0.5f;
				Vector3 b = new Vector3(GreebleUtility.ProceduralValue(-vector3.x, vector3.x), GreebleUtility.ProceduralValue(-vector3.y, vector3.y), GreebleUtility.ProceduralValue(-vector3.z, vector3.z));
				int num = 0;
				switch (this.Direction)
				{
				case GreebleRayDirection.Floor:
					num = 4;
					break;
				case GreebleRayDirection.Ceiling:
					num = 5;
					break;
				case GreebleRayDirection.Walls:
					num = GreebleUtility.ProceduralValue(0, 4);
					break;
				case GreebleRayDirection.AllDirections:
					num = GreebleUtility.ProceduralValue(0, 6);
					break;
				}
				switch (num)
				{
				case 0:
					vector2 = Vector3.left;
					b.x = vector3.x;
					distance = this.Size.x;
					break;
				case 1:
					vector2 = Vector3.right;
					b.x = -vector3.x;
					distance = this.Size.x;
					break;
				case 2:
					vector2 = Vector3.back;
					b.z = vector3.z;
					distance = this.Size.z;
					break;
				case 3:
					vector2 = Vector3.forward;
					b.z = -vector3.z;
					distance = this.Size.z;
					break;
				case 4:
					vector2 = Vector3.down;
					b.y = vector3.y;
					distance = this.Size.y;
					break;
				case 5:
					vector2 = Vector3.up;
					b.y = -vector3.y;
					distance = this.Size.y;
					break;
				}
				vector += b;
			}
		}
		else
		{
			distance = this.Radius;
			Vector3 zero = Vector3.zero;
			do
			{
				zero = new Vector3(GreebleUtility.ProceduralValue(-this.Radius, this.Radius), GreebleUtility.ProceduralValue(-this.Radius, this.Radius), GreebleUtility.ProceduralValue(-this.Radius, this.Radius));
			}
			while (zero.magnitude > this.Radius);
			switch (this.Direction)
			{
			case GreebleRayDirection.Floor:
				vector += new Vector3(zero.x, 0f, zero.y);
				vector2 = Vector3.down;
				break;
			case GreebleRayDirection.Ceiling:
				vector += new Vector3(zero.x, 0f, zero.y);
				vector2 = Vector3.up;
				break;
			case GreebleRayDirection.Walls:
				vector2 = GreebleUtility.ProceduralDirectionFast();
				vector2.y = 0f;
				vector2.Normalize();
				vector += zero - Vector3.Project(zero, vector2);
				break;
			case GreebleRayDirection.AllDirections:
				vector2 = GreebleUtility.ProceduralDirection();
				break;
			}
		}
		Quaternion rotation = Quaternion.identity;
		if (greebleDefinition.RandomizeRotation)
		{
			rotation = Quaternion.Euler((!greebleDefinition.AllowRotationX) ? 0f : GreebleUtility.ProceduralAngle(), (!greebleDefinition.AllowRotationY) ? 0f : GreebleUtility.ProceduralAngle(), (!greebleDefinition.AllowRotationZ) ? 0f : GreebleUtility.ProceduralAngle());
		}
		this.instances[index] = GreebleUtility.Spawn(greebleDefinition, new Ray(base.transform.TransformPoint(vector), base.transform.TransformDirection(vector2)), distance, rotation, 0.5f);
		if (this.Data != null && this.instances[index] != null && Application.isPlaying)
		{
			try
			{
				if (greebleDefinition.HasCustomActiveValue)
				{
					CustomActiveValueGreeble component = this.instances[index].GetComponent<CustomActiveValueGreeble>();
					component.Data = this.Data;
					component.Index = index;
					if (this.Data._instancesState[index] > 252)
					{
						this.Data._instancesState[index] = 252;
					}
				}
				else
				{
					this.Data._instancesState[index] = 253;
				}
			}
			catch (Exception message)
			{
				Debug.Log(message);
			}
		}
		if (this.instances[index])
		{
			if (this.MpSync && BoltNetwork.isServer)
			{
				BoltNetwork.Attach(this.instances[index]);
			}
			this.InstanceData[index].Spawned = true;
			if (this.OnSpawned != null)
			{
				this.OnSpawned(index, this.instances[index]);
			}
		}
		UnityEngine.Random.seed = this.GetRandomSeed();
	}

	public void SpawnAll()
	{
		this.Despawn();
		this.Start();
		UnityEngine.Random.seed = this.GetRandomSeed();
		this.InstanceCount = GreebleUtility.ProceduralValue(this.MinInstances, this.MaxInstances + 1);
		if (this.InstanceData == null || this.InstanceData.Length != this.InstanceCount)
		{
			this.InstanceData = new GreebleZone.GreebleInstanceData[this.InstanceCount];
			for (int i = 0; i < this.InstanceData.Length; i++)
			{
				this.InstanceData[i].Spawned = false;
				if (this.Data != null && Application.isPlaying)
				{
					this.InstanceData[i].Destroyed = (this.Data._instancesState[i] == 255);
				}
				else
				{
					this.InstanceData[i].Destroyed = false;
				}
				this.InstanceData[i].CreationTime = 0f;
			}
		}
		if (this.instances == null || this.instances.Length != this.InstanceCount)
		{
			this.instances = new GameObject[this.InstanceCount];
		}
		this.currentlyVisible = true;
		for (int j = 0; j < this.InstanceCount; j++)
		{
			this.SpawnIndex(j);
		}
		UnityEngine.Random.seed = this.GetRandomSeed();
	}

	public void Despawn()
	{
		if (this.instances == null || this.InstanceData == null)
		{
			return;
		}
		for (int i = 0; i < this.instances.Length; i++)
		{
			if (this.currentlyVisible && i < this.scheduledSpawnIndex && (!this.instances[i] || !this.instances[i].activeSelf) && this.InstanceData != null && this.InstanceData[i].Spawned)
			{
				if (this.AllowRegrowth)
				{
					this.InstanceData[i].CreationTime = Time.timeSinceLevelLoad;
				}
				else
				{
					this.InstanceData[i].Destroyed = true;
				}
				if (this.Data != null)
				{
					this.Data._instancesState[i] = 255;
				}
			}
			if (Application.isPlaying)
			{
				if (this.MpSync && BoltNetwork.isServer && this.instances[i])
				{
					BoltNetwork.Detach(this.instances[i]);
				}
				GreeblePlugin.Destroy(this.instances[i]);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(this.instances[i]);
			}
			this.InstanceData[i].Spawned = false;
			this.instances[i] = null;
		}
		this.currentlyVisible = false;
	}

	private void OnDestroy()
	{
		this.instances = null;
		this.InstanceData = null;
		this.Data = null;
		this.GreebleDefinitions = null;
		this.OnSpawned = null;
	}
}
