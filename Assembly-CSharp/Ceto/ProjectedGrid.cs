using Ceto.Common.Unity.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ceto
{
	[AddComponentMenu("Ceto/Components/ProjectedGrid"), DisallowMultipleComponent, RequireComponent(typeof(Ocean))]
	public class ProjectedGrid : OceanGridBase
	{
		public class Grid
		{
			public int screenWidth;

			public int screenHeight;

			public int resolution;

			public int groups;

			public IList<MeshFilter> topFilters = new List<MeshFilter>();

			public IList<Renderer> topRenderer = new List<Renderer>();

			public IList<GameObject> top = new List<GameObject>();

			public IList<MeshFilter> underFilters = new List<MeshFilter>();

			public IList<Renderer> underRenderer = new List<Renderer>();

			public IList<GameObject> under = new List<GameObject>();
		}

		private static readonly int MAX_SCREEN_WIDTH = 2048;

		private static readonly int MAX_SCREEN_HEIGHT = 2048;

		public MESH_RESOLUTION resolution = MESH_RESOLUTION.HIGH;

		private GRID_GROUPS gridGroups;

		public bool receiveShadows;

		private float borderLength = 200f;

		private ReflectionProbeUsage reflectionProbes;

		public Material oceanTopSideMat;

		public Material oceanUnderSideMat;

		private Dictionary<MESH_RESOLUTION, ProjectedGrid.Grid> m_grids = new Dictionary<MESH_RESOLUTION, ProjectedGrid.Grid>();

		private void Start()
		{
			try
			{
				if (SystemInfo.graphicsShaderLevel < 30)
				{
					throw new InvalidOperationException("The projected grids needs at least SM3 to render.");
				}
				if (this.oceanTopSideMat == null)
				{
					Ocean.LogWarning("Top side material is null. There will be no top ocean mesh rendered");
				}
				if (this.m_ocean.UnderWater != null && this.m_ocean.UnderWater.Mode == UNDERWATER_MODE.ABOVE_AND_BELOW && this.oceanUnderSideMat == null)
				{
					Ocean.LogWarning("Under side material is null. There will be no under ocean mesh rendered");
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (base.WasError)
			{
				return;
			}
			try
			{
				Dictionary<MESH_RESOLUTION, ProjectedGrid.Grid>.Enumerator enumerator = this.m_grids.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<MESH_RESOLUTION, ProjectedGrid.Grid> current = enumerator.Current;
					ProjectedGrid.Grid value = current.Value;
					this.Activate(value.top, true);
					this.Activate(value.under, true);
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			try
			{
				Dictionary<MESH_RESOLUTION, ProjectedGrid.Grid>.Enumerator enumerator = this.m_grids.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<MESH_RESOLUTION, ProjectedGrid.Grid> current = enumerator.Current;
					ProjectedGrid.Grid value = current.Value;
					this.Activate(value.top, false);
					this.Activate(value.under, false);
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		protected override void OnDestroy()
		{
			try
			{
				Dictionary<MESH_RESOLUTION, ProjectedGrid.Grid>.Enumerator enumerator = this.m_grids.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<MESH_RESOLUTION, ProjectedGrid.Grid> current = enumerator.Current;
					ProjectedGrid.Grid value = current.Value;
					this.ClearGrid(value);
				}
				this.m_grids.Clear();
				this.m_grids = null;
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		private void Update()
		{
			try
			{
				Shader.SetGlobalFloat("Ceto_GridEdgeBorder", Mathf.Max(0f, this.borderLength));
				int num = this.ResolutionToNumber(this.resolution);
				Vector2 v = new Vector2((float)num / (float)this.ScreenWidth(), (float)num / (float)this.ScreenHeight());
				Shader.SetGlobalVector("Ceto_ScreenGridSize", v);
				this.CreateGrid(this.resolution);
				Dictionary<MESH_RESOLUTION, ProjectedGrid.Grid>.Enumerator enumerator = this.m_grids.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<MESH_RESOLUTION, ProjectedGrid.Grid> current = enumerator.Current;
					ProjectedGrid.Grid value = current.Value;
					KeyValuePair<MESH_RESOLUTION, ProjectedGrid.Grid> current2 = enumerator.Current;
					bool flag = current2.Key == this.resolution;
					if (flag)
					{
						this.UpdateGrid(value);
						this.Activate(value.top, true);
						this.Activate(value.under, true);
					}
					else
					{
						this.Activate(value.top, false);
						this.Activate(value.under, false);
					}
				}
				if (this.m_ocean.UnderWater == null || this.m_ocean.UnderWater.Mode == UNDERWATER_MODE.ABOVE_ONLY)
				{
					enumerator = this.m_grids.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<MESH_RESOLUTION, ProjectedGrid.Grid> current3 = enumerator.Current;
						this.Activate(current3.Value.under, false);
					}
				}
				if (this.oceanTopSideMat != null && this.m_ocean.UnderWater != null && this.m_ocean.UnderWater.DepthMode == DEPTH_MODE.USE_DEPTH_BUFFER && this.oceanTopSideMat.shader.isSupported && this.oceanTopSideMat.renderQueue <= 2500)
				{
					Ocean.LogWarning("Underwater depth mode must be USE_OCEAN_DEPTH_PASS if using opaque material. Underwater effect will not look correct.");
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		public override void OceanOnPostRender(Camera cam, CameraData data)
		{
			if (!base.enabled || cam == null || data == null)
			{
				return;
			}
			ProjectedGrid.Grid grid = null;
			this.m_grids.TryGetValue(this.resolution, out grid);
			if (grid == null)
			{
				return;
			}
			this.ResetBounds(grid);
		}

		private void Activate(IList<GameObject> list, bool active)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (list[i] != null)
				{
					list[i].SetActive(active);
				}
			}
		}

		private void UpdateGrid(ProjectedGrid.Grid grid)
		{
			this.ResetBounds(grid);
			int count = grid.topRenderer.Count;
			for (int i = 0; i < count; i++)
			{
				grid.topRenderer[i].receiveShadows = this.receiveShadows;
				grid.topRenderer[i].reflectionProbeUsage = this.reflectionProbes;
				if (this.oceanTopSideMat != null)
				{
					grid.topRenderer[i].sharedMaterial = this.oceanTopSideMat;
				}
			}
			count = grid.underRenderer.Count;
			for (int j = 0; j < count; j++)
			{
				grid.underRenderer[j].receiveShadows = this.receiveShadows;
				grid.underRenderer[j].reflectionProbeUsage = this.reflectionProbes;
				if (this.oceanUnderSideMat != null)
				{
					grid.underRenderer[j].sharedMaterial = this.oceanUnderSideMat;
				}
			}
			count = grid.top.Count;
			for (int k = 0; k < count; k++)
			{
				grid.top[k].transform.localPosition = Vector3.zero;
				grid.top[k].transform.localRotation = Quaternion.identity;
				grid.top[k].transform.localScale = Vector3.one;
			}
			count = grid.under.Count;
			for (int l = 0; l < count; l++)
			{
				grid.under[l].transform.localPosition = Vector3.zero;
				grid.under[l].transform.localRotation = Quaternion.identity;
				grid.under[l].transform.localScale = Vector3.one;
			}
		}

		private void ResetBounds(ProjectedGrid.Grid grid)
		{
			float level = this.m_ocean.level;
			float num = this.m_ocean.FindMaxDisplacement(true);
			float num2 = 1E+08f;
			Bounds bounds = new Bounds(Vector3.zero, new Vector3(num2, level + num, num2));
			int count = grid.topFilters.Count;
			for (int i = 0; i < count; i++)
			{
				grid.topFilters[i].mesh.bounds = bounds;
			}
			count = grid.underFilters.Count;
			for (int j = 0; j < count; j++)
			{
				grid.underFilters[j].mesh.bounds = bounds;
			}
		}

		private void UpdateBounds(GameObject go, Camera cam)
		{
			MeshFilter component = go.GetComponent<MeshFilter>();
			if (component == null)
			{
				return;
			}
			Vector3 position = cam.transform.position;
			float level = this.m_ocean.level;
			float y = this.m_ocean.FindMaxDisplacement(true);
			float num = cam.farClipPlane * 2f;
			position.y = level;
			component.mesh.bounds = new Bounds(position, new Vector3(num, y, num));
		}

		private int ScreenWidth()
		{
			return Mathf.Min(Screen.width, ProjectedGrid.MAX_SCREEN_WIDTH);
		}

		private int ScreenHeight()
		{
			return Mathf.Min(Screen.height, ProjectedGrid.MAX_SCREEN_HEIGHT);
		}

		private void ApplyProjection(GameObject go)
		{
			try
			{
				if (base.enabled)
				{
					Camera current = Camera.current;
					if (!(current == null))
					{
						CameraData cameraData = this.m_ocean.FindCameraData(current);
						if (cameraData.projection == null)
						{
							cameraData.projection = new ProjectionData();
						}
						if (!cameraData.projection.updated)
						{
							this.m_ocean.Projection.UpdateProjection(current, cameraData, this.m_ocean.ProjectSceneView);
							Shader.SetGlobalMatrix("Ceto_Interpolation", cameraData.projection.interpolation);
							Shader.SetGlobalMatrix("Ceto_ProjectorVP", cameraData.projection.projectorVP);
						}
						if (!cameraData.projection.checkedForFlipping)
						{
							int num = 2;
							int num2 = 1;
							if (!Ocean.DISABLE_PROJECTION_FLIPPING)
							{
								bool isFlipped = this.m_ocean.Projection.IsFlipped;
								if (this.oceanTopSideMat != null)
								{
									this.oceanTopSideMat.SetInt("_CullFace", (!isFlipped) ? num : num2);
								}
								if (this.oceanUnderSideMat != null)
								{
									this.oceanUnderSideMat.SetInt("_CullFace", (!isFlipped) ? num2 : num);
								}
							}
							else
							{
								if (this.oceanTopSideMat != null)
								{
									this.oceanTopSideMat.SetInt("_CullFace", num);
								}
								if (this.oceanUnderSideMat != null)
								{
									this.oceanUnderSideMat.SetInt("_CullFace", num2);
								}
							}
							cameraData.projection.checkedForFlipping = true;
						}
						this.UpdateBounds(go, current);
					}
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		private void CreateGrid(MESH_RESOLUTION meshRes)
		{
			ProjectedGrid.Grid grid = null;
			if (!this.m_grids.TryGetValue(meshRes, out grid))
			{
				grid = new ProjectedGrid.Grid();
				this.m_grids.Add(meshRes, grid);
			}
			int num = this.ScreenWidth();
			int num2 = this.ScreenHeight();
			int num3 = this.ResolutionToNumber(meshRes);
			int num4 = this.ChooseGroupSize(num3, this.gridGroups, num, num2);
			if (!base.ForceRecreate && grid.screenWidth == num && grid.screenHeight == num2 && grid.groups == num4)
			{
				return;
			}
			this.ClearGrid(grid);
			grid.screenWidth = num;
			grid.screenHeight = num2;
			grid.resolution = num3;
			grid.groups = num4;
			base.ForceRecreate = false;
			IList<Mesh> list = this.CreateScreenQuads(num3, num4, num, num2);
			foreach (Mesh current in list)
			{
				if (this.oceanTopSideMat != null)
				{
					GameObject gameObject = new GameObject("Ceto TopSide Grid LOD: " + meshRes);
					MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
					MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
					NotifyOnWillRender notifyOnWillRender = gameObject.AddComponent<NotifyOnWillRender>();
					meshFilter.sharedMesh = current;
					meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
					meshRenderer.receiveShadows = this.receiveShadows;
					meshRenderer.sharedMaterial = this.oceanTopSideMat;
					meshRenderer.reflectionProbeUsage = this.reflectionProbes;
					gameObject.layer = LayerMask.NameToLayer(Ocean.OCEAN_LAYER);
					gameObject.hideFlags = HideFlags.HideAndDontSave;
					notifyOnWillRender.AddAction(new Action<GameObject>(this.m_ocean.RenderReflection));
					notifyOnWillRender.AddAction(new Action<GameObject>(this.ApplyProjection));
					notifyOnWillRender.AddAction(new Action<GameObject>(this.m_ocean.RenderWaveOverlays));
					notifyOnWillRender.AddAction(new Action<GameObject>(this.m_ocean.RenderOceanMask));
					notifyOnWillRender.AddAction(new Action<GameObject>(this.m_ocean.RenderOceanDepth));
					grid.top.Add(gameObject);
					grid.topRenderer.Add(meshRenderer);
					grid.topFilters.Add(meshFilter);
				}
				if (this.oceanUnderSideMat)
				{
					GameObject gameObject2 = new GameObject("Ceto UnderSide Grid LOD: " + meshRes);
					MeshFilter meshFilter2 = gameObject2.AddComponent<MeshFilter>();
					MeshRenderer meshRenderer2 = gameObject2.AddComponent<MeshRenderer>();
					NotifyOnWillRender notifyOnWillRender2 = gameObject2.AddComponent<NotifyOnWillRender>();
					meshFilter2.sharedMesh = current;
					meshRenderer2.shadowCastingMode = ShadowCastingMode.Off;
					meshRenderer2.receiveShadows = this.receiveShadows;
					meshRenderer2.reflectionProbeUsage = this.reflectionProbes;
					meshRenderer2.sharedMaterial = this.oceanUnderSideMat;
					gameObject2.layer = LayerMask.NameToLayer(Ocean.OCEAN_LAYER);
					gameObject2.hideFlags = HideFlags.HideAndDontSave;
					notifyOnWillRender2.AddAction(new Action<GameObject>(this.ApplyProjection));
					notifyOnWillRender2.AddAction(new Action<GameObject>(this.m_ocean.RenderWaveOverlays));
					notifyOnWillRender2.AddAction(new Action<GameObject>(this.m_ocean.RenderOceanMask));
					notifyOnWillRender2.AddAction(new Action<GameObject>(this.m_ocean.RenderOceanDepth));
					grid.under.Add(gameObject2);
					grid.underRenderer.Add(meshRenderer2);
					grid.underFilters.Add(meshFilter2);
				}
				UnityEngine.Object.Destroy(current);
			}
		}

		private void ClearGrid(ProjectedGrid.Grid grid)
		{
			if (grid == null)
			{
				return;
			}
			grid.screenWidth = 0;
			grid.screenHeight = 0;
			grid.resolution = 0;
			grid.groups = 0;
			if (grid.top != null)
			{
				int count = grid.top.Count;
				for (int i = 0; i < count; i++)
				{
					if (!(grid.top[i] == null))
					{
						UnityEngine.Object.Destroy(grid.top[i]);
					}
				}
				grid.top.Clear();
			}
			if (grid.topFilters != null)
			{
				int count2 = grid.topFilters.Count;
				for (int j = 0; j < count2; j++)
				{
					if (!(grid.topFilters[j] == null))
					{
						UnityEngine.Object.Destroy(grid.topFilters[j].mesh);
					}
				}
				grid.topFilters.Clear();
			}
			if (grid.under != null)
			{
				int count3 = grid.under.Count;
				for (int k = 0; k < count3; k++)
				{
					if (!(grid.under[k] == null))
					{
						UnityEngine.Object.Destroy(grid.under[k]);
					}
				}
				grid.under.Clear();
			}
			if (grid.underFilters != null)
			{
				int count4 = grid.underFilters.Count;
				for (int l = 0; l < count4; l++)
				{
					if (!(grid.underFilters[l] == null))
					{
						UnityEngine.Object.Destroy(grid.underFilters[l].mesh);
					}
				}
				grid.underFilters.Clear();
			}
			if (grid.topRenderer != null)
			{
				grid.topRenderer.Clear();
			}
			if (grid.underRenderer != null)
			{
				grid.underRenderer.Clear();
			}
		}

		private int ResolutionToNumber(MESH_RESOLUTION resolution)
		{
			switch (resolution)
			{
			case MESH_RESOLUTION.LOW:
				return 16;
			case MESH_RESOLUTION.MEDIUM:
				return 8;
			case MESH_RESOLUTION.HIGH:
				return 4;
			case MESH_RESOLUTION.ULTRA:
				return 2;
			case MESH_RESOLUTION.EXTREME:
				return 1;
			default:
				return 16;
			}
		}

		private int GroupToNumber(GRID_GROUPS groups)
		{
			switch (groups)
			{
			case GRID_GROUPS.SINGLE:
				return -1;
			case GRID_GROUPS.LOW:
				return 512;
			case GRID_GROUPS.MEDIUM:
				return 256;
			case GRID_GROUPS.HIGH:
				return 196;
			case GRID_GROUPS.EXTREME:
				return 128;
			default:
				return 128;
			}
		}

		private int ChooseGroupSize(int resolution, GRID_GROUPS groups, int width, int height)
		{
			int num = this.GroupToNumber(groups);
			int num2;
			int num3;
			if (num == -1)
			{
				num2 = width / resolution;
				num3 = height / resolution;
			}
			else
			{
				num2 = num / resolution;
				num3 = num / resolution;
			}
			while (num2 * num3 > 65000)
			{
				if (groups == GRID_GROUPS.EXTREME)
				{
					throw new InvalidOperationException("Can not increase group size");
				}
				int num4 = (int)(groups + 1);
				groups = (GRID_GROUPS)num4;
				num = this.GroupToNumber(groups);
				num2 = num / resolution;
				num3 = num / resolution;
			}
			return num;
		}

		private IList<Mesh> CreateScreenQuads(int resolution, int groupSize, int width, int height)
		{
			int numVertsX;
			int numVertsY;
			int num;
			int num2;
			float num3;
			float num4;
			if (groupSize != -1)
			{
				while (width % groupSize != 0)
				{
					width++;
				}
				while (height % groupSize != 0)
				{
					height++;
				}
				numVertsX = groupSize / resolution;
				numVertsY = groupSize / resolution;
				num = width / groupSize;
				num2 = height / groupSize;
				num3 = (float)groupSize / (float)width;
				num4 = (float)groupSize / (float)height;
			}
			else
			{
				numVertsX = width / resolution;
				numVertsY = height / resolution;
				num = 1;
				num2 = 1;
				num3 = 1f;
				num4 = 1f;
			}
			List<Mesh> list = new List<Mesh>();
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					float ux = (float)i * num3;
					float uy = (float)j * num4;
					Mesh item = this.CreateQuad(numVertsX, numVertsY, ux, uy, num3, num4);
					list.Add(item);
				}
			}
			return list;
		}

		public Mesh CreateQuad(int numVertsX, int numVertsY, float ux, float uy, float w, float h)
		{
			Vector3[] array = new Vector3[numVertsX * numVertsY];
			Vector2[] array2 = new Vector2[numVertsX * numVertsY];
			int[] array3 = new int[numVertsX * numVertsY * 6];
			float num = 0.1f;
			for (int i = 0; i < numVertsX; i++)
			{
				for (int j = 0; j < numVertsY; j++)
				{
					Vector2 vector = new Vector3((float)i / (float)(numVertsX - 1), (float)j / (float)(numVertsY - 1));
					vector.x *= w;
					vector.x += ux;
					vector.y *= h;
					vector.y += uy;
					if (!Ocean.DISABLE_PROJECTED_GRID_BORDER)
					{
						vector.x = vector.x * (1f + num * 2f) - num;
						vector.y = vector.y * (1f + num * 2f) - num;
						Vector2 vector2 = vector;
						vector2.x = Mathf.Clamp01(vector2.x);
						vector2.y = Mathf.Clamp01(vector2.y);
						Vector2 vector3 = vector;
						if (vector3.x < 0f)
						{
							vector3.x = Mathf.Abs(vector3.x) / num;
						}
						else if (vector3.x > 1f)
						{
							vector3.x = Mathf.Max(0f, vector3.x - 1f) / num;
						}
						else
						{
							vector3.x = 0f;
						}
						if (vector3.y < 0f)
						{
							vector3.y = Mathf.Abs(vector3.y) / num;
						}
						else if (vector3.y > 1f)
						{
							vector3.y = Mathf.Max(0f, vector3.y - 1f) / num;
						}
						else
						{
							vector3.y = 0f;
						}
						vector3.x = Mathf.Pow(vector3.x, 2f);
						vector3.y = Mathf.Pow(vector3.y, 2f);
						array2[i + j * numVertsX] = vector3;
						array[i + j * numVertsX] = new Vector3(vector2.x, vector2.y, 0f);
					}
					else
					{
						array2[i + j * numVertsX] = new Vector2(0f, 0f);
						array[i + j * numVertsX] = new Vector3(vector.x, vector.y, 0f);
					}
				}
			}
			int num2 = 0;
			for (int k = 0; k < numVertsX - 1; k++)
			{
				for (int l = 0; l < numVertsY - 1; l++)
				{
					array3[num2++] = k + l * numVertsX;
					array3[num2++] = k + (l + 1) * numVertsX;
					array3[num2++] = k + 1 + l * numVertsX;
					array3[num2++] = k + (l + 1) * numVertsX;
					array3[num2++] = k + 1 + (l + 1) * numVertsX;
					array3[num2++] = k + 1 + l * numVertsX;
				}
			}
			Mesh mesh = new Mesh();
			mesh.vertices = array;
			mesh.uv = array2;
			mesh.triangles = array3;
			mesh.name = "Ceto Projected Grid Mesh";
			mesh.hideFlags = HideFlags.HideAndDontSave;
			mesh.Optimize();
			return mesh;
		}
	}
}
