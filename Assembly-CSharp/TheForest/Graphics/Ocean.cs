using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TheForest.Graphics
{
	[AddComponentMenu("The Forest/Graphics/Ocean"), ExecuteInEditMode]
	public class Ocean : Water
	{
		[Serializable]
		public class MeshLOD
		{
			public float distance = 10000f;

			public int triangles = 130050;
		}

		public Material meshMaterial;

		public List<Ocean.MeshLOD> meshLOD = new List<Ocean.MeshLOD>();

		[SerializeField]
		private List<GameObject> meshObject = new List<GameObject>();

		public float level = 41.5f;

		[SerializeField]
		private bool debug;

		public List<GameObject> MeshObject
		{
			get
			{
				return this.meshObject;
			}
		}

		public override Material SharedMaterial
		{
			get
			{
				return this.meshMaterial;
			}
		}

		public override Material InstanceMaterial
		{
			get
			{
				return this.meshMaterial;
			}
		}

		private void EnableMesh()
		{
			this.DisableMesh();
			float num = 0f;
			for (int i = 0; i < this.meshLOD.Count; i++)
			{
				GameObject gameObject = new GameObject("Ocean LOD " + i);
				gameObject.hideFlags = ((!Application.isPlaying) ? HideFlags.HideAndDontSave : HideFlags.DontSave);
				gameObject.layer = LayerMask.NameToLayer("Water");
				this.meshObject.Add(gameObject);
				MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
				meshFilter.sharedMesh = this.CreateMesh(num, this.meshLOD[i].distance, 1f, (int)Mathf.Sqrt((float)this.meshLOD[i].triangles / 2f));
				MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = this.meshMaterial;
				meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				meshRenderer.receiveShadows = false;
				meshRenderer.useLightProbes = false;
				meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
				num += this.meshLOD[i].distance;
			}
			if (this.meshObject.Count > 0)
			{
				this.meshObject[this.meshObject.Count - 1].AddComponent<RenderProxy>();
			}
		}

		private void DisableMesh()
		{
			for (int i = 0; i < this.meshObject.Count; i++)
			{
				if (this.meshObject[i])
				{
					this.DestroyMesh(this.meshObject[i]);
				}
				this.meshObject[i] = null;
			}
			this.meshObject.Clear();
		}

		private Mesh CreateMesh(float r0, float scale, float power, int resolution)
		{
			Vector3[] array = new Vector3[resolution * resolution];
			Vector3[] array2 = new Vector3[resolution * resolution];
			Vector2[] array3 = new Vector2[resolution * resolution];
			int[] array4 = new int[resolution * resolution * 6];
			float num = 6.28318548f;
			for (int i = 0; i < resolution; i++)
			{
				for (int j = 0; j < resolution; j++)
				{
					float num2 = r0 + Mathf.Pow((float)i / (float)(resolution - 1), power) * scale;
					array[i + j * resolution].x = num2 * Mathf.Cos(num * (float)j / (float)(resolution - 1));
					array[i + j * resolution].y = 0f;
					array[i + j * resolution].z = num2 * Mathf.Sin(num * (float)j / (float)(resolution - 1));
					array2[i + j * resolution] = new Vector3(0f, 1f, 0f);
					array3[i + j * resolution].x = array[i + j * resolution].x;
					array3[i + j * resolution].y = array[i + j * resolution].z;
				}
			}
			int num3 = 0;
			for (int k = 0; k < resolution - 1; k++)
			{
				for (int l = 0; l < resolution - 1; l++)
				{
					array4[num3++] = k + l * resolution;
					array4[num3++] = k + (l + 1) * resolution;
					array4[num3++] = k + 1 + l * resolution;
					array4[num3++] = k + (l + 1) * resolution;
					array4[num3++] = k + 1 + (l + 1) * resolution;
					array4[num3++] = k + 1 + l * resolution;
				}
			}
			Mesh mesh = new Mesh();
			mesh.name = "Radial Mesh";
			mesh.hideFlags = HideFlags.DontSave;
			mesh.vertices = array;
			mesh.uv = array3;
			mesh.normals = array2;
			mesh.triangles = array4;
			mesh.RecalculateBounds();
			mesh.bounds = new Bounds(mesh.bounds.center, Vector3.one * 200000f);
			return mesh;
		}

		private void DestroyMesh(GameObject gameObject)
		{
			MeshFilter component = gameObject.GetComponent<MeshFilter>();
			if (component)
			{
				Mesh sharedMesh = component.sharedMesh;
				component.sharedMesh = null;
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(sharedMesh);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(sharedMesh);
				}
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(component);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(component);
				}
			}
			MeshRenderer component2 = gameObject.GetComponent<MeshRenderer>();
			if (component2)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(component2);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(component2);
				}
			}
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(gameObject);
			}
		}

		private void UpdateMeshTransform(Vector3 position)
		{
			for (int i = 0; i < this.meshObject.Count; i++)
			{
				if (this.meshObject[i])
				{
					this.meshObject[i].transform.position = position;
				}
			}
		}

		[ContextMenu("DEBUG ON")]
		private void DebugOn()
		{
			this.debug = true;
			this.DebugFlags();
		}

		[ContextMenu("DEBUG OFF")]
		private void DebugOff()
		{
			this.debug = false;
			this.DebugFlags();
		}

		private void DebugFlags()
		{
			for (int i = 0; i < this.meshObject.Count; i++)
			{
				this.meshObject[i].hideFlags = ((!this.debug) ? HideFlags.HideAndDontSave : HideFlags.DontSave);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			WaterEngine.Ocean = this;
			this.EnableMesh();
			this.DebugFlags();
			this.UpdateTransform(Vector3.zero);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			WaterEngine.Ocean = null;
			this.DisableMesh();
		}

		protected override void Update()
		{
			base.Update();
			if (WaterEngine.Camera)
			{
				this.UpdateTransform(WaterEngine.CameraTransform.position);
			}
		}

		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
		}

		private void UpdateTransform(Vector3 position)
		{
			position.y = this.level;
			base.transform.position = position;
			this.UpdateMeshTransform(position);
		}
	}
}
