using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using TheForest.World;
using UniLinq;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
	private const float DAMPFER = 0.15f;

	private const float WATER_DENSITY = 1000f;

	public bool isPlayer;

	public bool CheckPlayerDiving;

	public bool ForceValidateTriggers;

	public int inWaterCounter;

	private Collider lastWaterCollider;

	public bool IsOcean;

	public bool ResetVelocityOnExitWater;

	public bool SkipAwakeVoxelise;

	public float density = 500f;

	public int slicesPerAxis = 2;

	public int voxelsLimit = 16;

	private float lastWaterHeight;

	[HideInInspector]
	public float voxelHalfHeight;

	[HideInInspector]
	public Vector3 localArchimedesForce;

	[HideInInspector]
	public Vector3[] Voxels;

	private static readonly Vector3[] _directions = new Vector3[]
	{
		Vector3.up,
		Vector3.down,
		Vector3.left,
		Vector3.right,
		Vector3.forward,
		Vector3.back
	};

	private List<Collider> WaterTriggers = new List<Collider>();

	public bool InWater
	{
		get
		{
			return base.enabled && this.inWaterCounter > 0;
		}
	}

	public float WaterLevel
	{
		get
		{
			return (!this.InWater) ? -99999f : this.lastWaterHeight;
		}
	}

	public Collider WaterCollider
	{
		get
		{
			return this.lastWaterCollider;
		}
	}

	public float LastWaterEnterSpeed
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (!this.SkipAwakeVoxelise && (this.Voxels == null || this.Voxels.Length <= 0))
		{
			UnityEngine.Debug.LogWarning("Buoyant object not voxelized... Doing this at runtime can be slow!");
			this.Voxelize(base.transform);
		}
	}

	public void Voxelize(Transform collidersRoot)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(base.gameObject);
			Buoyancy component = gameObject.GetComponent<Buoyancy>();
			component.Voxelize(component.transform);
			if (component.Voxels != null && component.Voxels.Length > 0)
			{
				this.Voxels = new Vector3[component.Voxels.Length];
				component.Voxels.CopyTo(this.Voxels, 0);
				this.voxelHalfHeight = component.voxelHalfHeight;
				this.localArchimedesForce = component.localArchimedesForce;
			}
			UnityEngine.Object.DestroyImmediate(gameObject);
			return;
		}
		Collider[] componentsInChildren = collidersRoot.GetComponentsInChildren<Collider>();
		bool flag = false;
		Bounds bounds = default(Bounds);
		Collider[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Collider collider = array[i];
			if (collider.enabled && !collider.isTrigger)
			{
				bounds = collider.bounds;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			if (Application.isPlaying)
			{
				base.enabled = false;
			}
			UnityEngine.Debug.LogWarning(string.Format("[Buoyancy.cs] Object \"{0}\" had no collider.", base.name));
		}
		else
		{
			Quaternion rotation = collidersRoot.rotation;
			Vector3 position = collidersRoot.position;
			collidersRoot.rotation = Quaternion.identity;
			collidersRoot.position = Vector3.zero;
			if (componentsInChildren.Length > 1)
			{
				Vector3 min = new Vector3(3.40282347E+38f, 3.40282347E+38f, 3.40282347E+38f);
				Vector3 max = new Vector3(-3.40282347E+38f, -3.40282347E+38f, -3.40282347E+38f);
				for (int j = componentsInChildren.Length - 1; j >= 0; j--)
				{
					Collider collider2 = componentsInChildren[j];
					if (collider2.enabled && !collider2.isTrigger && collider2.gameObject.activeInHierarchy)
					{
						Vector3 min2 = collider2.bounds.min;
						Vector3 max2 = collider2.bounds.max;
						if (min2.x < min.x)
						{
							min.x = min2.x;
						}
						if (min2.y < min.y)
						{
							min.y = min2.y;
						}
						if (min2.z < min.z)
						{
							min.z = min2.z;
						}
						if (max2.x > max.x)
						{
							max.x = max2.x;
						}
						if (max2.y > max.y)
						{
							max.y = max2.y;
						}
						if (max2.z > max.z)
						{
							max.z = max2.z;
						}
					}
				}
				bounds.min = min;
				bounds.max = max;
			}
			if (bounds.size.x < bounds.size.y)
			{
				this.voxelHalfHeight = bounds.size.x;
			}
			else
			{
				this.voxelHalfHeight = bounds.size.y;
			}
			if (bounds.size.z < this.voxelHalfHeight)
			{
				this.voxelHalfHeight = bounds.size.z;
			}
			this.voxelHalfHeight /= (float)(2 * this.slicesPerAxis);
			base.GetComponent<Rigidbody>().centerOfMass = new Vector3(0f, -bounds.extents.y * 0.2f, 0f) + bounds.center;
			List<Vector3> list = this.SliceIntoVoxels(collidersRoot, componentsInChildren, bounds);
			collidersRoot.position = position;
			collidersRoot.rotation = rotation;
			float num = base.GetComponent<Rigidbody>().mass / this.density;
			Buoyancy.WeldPoints(list, this.voxelsLimit);
			float y = 1000f * Mathf.Abs(Physics.gravity.y) * num;
			this.localArchimedesForce = new Vector3(0f, y, 0f) / (float)list.Count;
			this.Voxels = list.ToArray();
		}
	}

	private List<Vector3> SliceIntoVoxels(Transform collidersRoot, Collider[] colliders, Bounds bounds)
	{
		List<Vector3> list = new List<Vector3>(this.slicesPerAxis * this.slicesPerAxis * this.slicesPerAxis);
		Vector3 min = bounds.min;
		Vector3 vector = bounds.size / ((float)this.slicesPerAxis - 0.6f);
		for (int i = 0; i < this.slicesPerAxis; i++)
		{
			for (int j = 0; j < this.slicesPerAxis; j++)
			{
				for (int k = 0; k < this.slicesPerAxis; k++)
				{
					Vector3 vector2 = new Vector3(min.x + vector.x * (0.2f + (float)i), min.y + vector.y * (0.2f + (float)j), min.z + vector.z * (0.2f + (float)k));
					for (int l = 0; l < colliders.Length; l++)
					{
						Collider collider = colliders[l];
						if (collider.enabled && !collider.isTrigger)
						{
							Vector3 vector3 = collider.transform.InverseTransformPoint(vector2);
							MeshCollider meshCollider = collider as MeshCollider;
							SphereCollider sphereCollider = collider as SphereCollider;
							bool flag;
							if (meshCollider)
							{
								bool convex = meshCollider.convex;
								meshCollider.convex = true;
								flag = Buoyancy.PointInsideMeshCollider(meshCollider, vector3);
								meshCollider.convex = convex;
							}
							else if (sphereCollider)
							{
								float radius = sphereCollider.radius;
								flag = ((vector3 - sphereCollider.center).sqrMagnitude < radius * radius);
							}
							else
							{
								flag = collider.bounds.Contains(vector2);
							}
							if (flag)
							{
								list.Add((!(collidersRoot == collider.transform)) ? collidersRoot.InverseTransformPoint(vector2) : vector3);
								break;
							}
						}
					}
				}
			}
		}
		return list;
	}

	private static bool PointInsideMeshCollider(Collider c, Vector3 p)
	{
		Vector3[] directions = Buoyancy._directions;
		for (int i = 0; i < directions.Length; i++)
		{
			Vector3 vector = directions[i];
			RaycastHit raycastHit;
			if (!c.Raycast(new Ray(p - vector * 1000f, vector), out raycastHit, 1000f))
			{
				return false;
			}
		}
		return true;
	}

	private static void FindClosestPoints(IList<Vector3> list, out int firstIndex, out int secondIndex)
	{
		float num = 3.40282347E+38f;
		float num2 = -3.40282347E+38f;
		firstIndex = 0;
		secondIndex = 1;
		for (int i = 0; i < list.Count - 1; i++)
		{
			for (int j = i + 1; j < list.Count; j++)
			{
				float num3 = Vector3.Distance(list[i], list[j]);
				if (num3 < num)
				{
					num = num3;
					firstIndex = i;
					secondIndex = j;
				}
				if (num3 > num2)
				{
					num2 = num3;
				}
			}
		}
	}

	private static void WeldPoints(IList<Vector3> list, int targetCount)
	{
		if (list.Count <= 2 || targetCount < 2)
		{
			return;
		}
		while (list.Count > targetCount)
		{
			int index;
			int index2;
			Buoyancy.FindClosestPoints(list, out index, out index2);
			Vector3 item = (list[index] + list[index2]) * 0.5f;
			list.RemoveAt(index2);
			list.RemoveAt(index);
			list.Add(item);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Water"))
		{
			if (this.isPlayer && LocalPlayer.AnimControl.playerHeadCollider && LocalPlayer.AnimControl.playerHeadCollider.enabled && other.enabled)
			{
				Physics.IgnoreCollision(LocalPlayer.AnimControl.playerHeadCollider, other, true);
			}
			if (!this.WaterTriggers.Contains(other))
			{
				this.inWaterCounter++;
				this.WaterTriggers.Add(other);
				this.IsOcean = other.GetComponent<IsOcean>();
				if (this.lastWaterCollider == null || this.lastWaterCollider.bounds.max.y < other.bounds.max.y)
				{
					this.lastWaterCollider = other;
					this.lastWaterHeight = this.lastWaterCollider.bounds.max.y;
				}
				Rigidbody component = base.GetComponent<Rigidbody>();
				if (component)
				{
					this.LastWaterEnterSpeed = component.velocity.magnitude;
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Water") && this.WaterTriggers.Contains(other))
		{
			this.inWaterCounter--;
			this.WaterTriggers.Remove(other);
			if (this.WaterTriggers.Count > 0 && (!this.lastWaterCollider || this.lastWaterCollider.Equals(other)))
			{
				this.lastWaterCollider = this.WaterTriggers.FirstOrDefault<Collider>();
				if (this.lastWaterCollider != null)
				{
					this.lastWaterHeight = this.lastWaterCollider.bounds.max.y;
				}
			}
			else
			{
				this.lastWaterCollider = null;
				if (this.ResetVelocityOnExitWater)
				{
					base.StartCoroutine(this.ResetRigidbody());
				}
			}
		}
	}

	[DebuggerHidden]
	public IEnumerator ResetRigidbody()
	{
		Buoyancy.<ResetRigidbody>c__Iterator15A <ResetRigidbody>c__Iterator15A = new Buoyancy.<ResetRigidbody>c__Iterator15A();
		<ResetRigidbody>c__Iterator15A.<>f__this = this;
		return <ResetRigidbody>c__Iterator15A;
	}

	private void ValidateTriggers()
	{
		if (this.WaterTriggers.Count > 0)
		{
			Bounds bounds = base.transform.GetComponent<Collider>().bounds;
			for (int i = this.WaterTriggers.Count - 1; i >= 0; i--)
			{
				if (!this.WaterTriggers[i])
				{
					this.inWaterCounter--;
					this.WaterTriggers.RemoveAt(i);
					if (!this.lastWaterCollider)
					{
						this.lastWaterCollider = this.WaterTriggers.FirstOrDefault<Collider>();
						if (this.lastWaterCollider != null)
						{
							this.lastWaterHeight = this.lastWaterCollider.bounds.max.y;
						}
					}
				}
				else if (!this.WaterTriggers[i].bounds.Intersects(bounds))
				{
					this.OnTriggerExit(this.WaterTriggers[i]);
				}
			}
			if (!this.lastWaterCollider && this.WaterTriggers.Count > 0)
			{
				this.lastWaterCollider = this.WaterTriggers[0];
			}
		}
	}

	private void FixedUpdate()
	{
		if (this.InWater && (this.CheckPlayerDiving || this.ForceValidateTriggers))
		{
			this.ValidateTriggers();
		}
		if (!this.InWater || base.GetComponent<Rigidbody>().IsSleeping() || (this.CheckPlayerDiving && LocalPlayer.FpCharacter.Diving))
		{
			return;
		}
		Rigidbody component = base.GetComponent<Rigidbody>();
		float waterLevel = this.WaterLevel;
		for (int i = 0; i < this.Voxels.Length; i++)
		{
			Vector3 position = this.Voxels[i];
			Vector3 vector = base.transform.TransformPoint(position);
			if (vector.y - this.voxelHalfHeight < waterLevel)
			{
				float num = (waterLevel - vector.y) / (2f * this.voxelHalfHeight) + 0.5f;
				if (num > 1f)
				{
					num = 1f;
				}
				else if (num < 0f)
				{
					num = 0f;
				}
				Vector3 pointVelocity = component.GetPointVelocity(vector);
				Vector3 a = -pointVelocity * 0.15f * component.mass;
				Vector3 force = a + Mathf.Sqrt(num) * this.localArchimedesForce;
				if (this.isPlayer && base.transform.position.y + 0.5f < waterLevel)
				{
					component.AddForceAtPosition(force, vector);
				}
				else if (!this.isPlayer)
				{
					component.AddForceAtPosition(force, vector);
				}
			}
		}
	}
}
