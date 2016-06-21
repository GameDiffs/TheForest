using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheForest.Utils
{
	public class CullingGrid : MonoBehaviour
	{
		public class Cell
		{
			public bool _enabled;

			public List<Renderer> _renderers = new List<Renderer>();
		}

		public int _gridSize = 25;

		public int _gridWorldSize = 35;

		public float _circleRadius = 7f;

		public float _circleOffset = 3f;

		public float _circle2Radius = 2.5f;

		public float _circle2Offset = -1f;

		[Header("Gizmos")]
		public bool _showGrid;

		public bool _showRendererDensity;

		public float _highestCellRendererCount;

		private static CullingGrid Instance;

		private Vector3 _offset;

		private CullingGrid.Cell[,] _rendererGrid;

		private Vector2 _prevCirclePosition;

		private Vector2 _prevCircle2Position;

		private Vector2 _circlePosition;

		private Vector2 _circle2Position;

		private void Awake()
		{
			if (CullingGrid.Instance != this)
			{
				CullingGrid.Instance = this;
				this._offset = new Vector3((float)(-(float)this._gridSize / 2 * this._gridWorldSize), 0f, (float)(-(float)this._gridSize / 2 * this._gridWorldSize));
				this._rendererGrid = new CullingGrid.Cell[this._gridSize, this._gridSize];
				for (int i = 0; i < this._gridSize; i++)
				{
					for (int j = 0; j < this._gridSize; j++)
					{
						this._rendererGrid[i, j] = new CullingGrid.Cell();
					}
				}
				this._circlePosition = Vector2.zero;
				this._circle2Position = Vector2.zero;
			}
		}

		private void Update()
		{
			if (LocalPlayer.Transform)
			{
				float finalRadius = this.GetFinalRadius();
				float circle2Radius = this._circle2Radius;
				float x = Mathf.Min(this._circlePosition.x - finalRadius - 1f, this._circle2Position.x - circle2Radius - 1f);
				float num = Mathf.Max(this._circlePosition.x + finalRadius + 1f, this._circle2Position.x + circle2Radius + 1f);
				float y = Mathf.Min(this._circlePosition.y - finalRadius - 1f, this._circle2Position.y - circle2Radius - 1f);
				float num2 = Mathf.Max(this._circlePosition.y + finalRadius + 1f, this._circle2Position.y + circle2Radius + 1f);
				Vector2 a = new Vector2(x, y);
				while (a.x < num)
				{
					a.y = y;
					while (a.y < num2)
					{
						if (a.x >= 0f && a.x < (float)this._gridSize && a.y >= 0f && a.y < (float)this._gridSize)
						{
							bool flag = Vector2.Distance(a, this._circlePosition) <= finalRadius || Vector2.Distance(a, this._circle2Position) <= circle2Radius;
							CullingGrid.Cell cell = this._rendererGrid[Mathf.FloorToInt(a.x), Mathf.FloorToInt(a.y)];
							if (cell._enabled != flag)
							{
								bool enabled = flag;
								List<Renderer> renderers = cell._renderers;
								int count = renderers.Count;
								for (int i = 0; i < count; i++)
								{
									renderers[i].enabled = enabled;
								}
								cell._enabled = enabled;
							}
						}
						a.y += 1f;
					}
					a.x += 1f;
				}
				this._prevCirclePosition = this._circlePosition;
				this._prevCircle2Position = this._circle2Position;
				Vector3 from = LocalPlayer.Transform.position + LocalPlayer.Transform.forward * this._circleOffset * (float)this._gridWorldSize;
				this._circlePosition.x = (float)this.WorldToGridXRounded(from.x);
				this._circlePosition.y = (float)this.WorldToGridYRounded(from.z);
				Vector3 a2 = (!Clock.Dark) ? Scene.Atmosphere.Sun.transform.forward : Scene.Atmosphere.Moon.transform.forward;
				a2.y = 0f;
				Vector3 a3 = Vector3.Lerp(from, LocalPlayer.Transform.position, a2.magnitude);
				Vector3 vector = a3 + a2 * this._circle2Offset * (float)this._gridWorldSize;
				this._circle2Position.x = (float)this.WorldToGridXRounded(vector.x);
				this._circle2Position.y = (float)this.WorldToGridYRounded(vector.z);
			}
		}

		private void OnEnable()
		{
			for (int i = 0; i < this._gridSize; i++)
			{
				for (int j = 0; j < this._gridSize; j++)
				{
					CullingGrid.Cell cell = this._rendererGrid[i, j];
					if (cell._enabled)
					{
						List<Renderer> renderers = cell._renderers;
						int count = renderers.Count;
						for (int k = 0; k < count; k++)
						{
							renderers[k].enabled = false;
						}
						cell._enabled = false;
					}
				}
			}
		}

		private void OnDisable()
		{
			for (int i = 0; i < this._gridSize; i++)
			{
				for (int j = 0; j < this._gridSize; j++)
				{
					CullingGrid.Cell cell = this._rendererGrid[i, j];
					if (!cell._enabled)
					{
						List<Renderer> renderers = cell._renderers;
						int count = renderers.Count;
						for (int k = 0; k < count; k++)
						{
							renderers[k].enabled = true;
						}
						cell._enabled = true;
					}
				}
			}
		}

		private void OnDestroy()
		{
			if (CullingGrid.Instance == this)
			{
				CullingGrid.Instance = null;
			}
		}

		public static int Register(Renderer r)
		{
			return CullingGrid.Instance.RegisterInternal(r);
		}

		public static void Unregister(Renderer r, int token)
		{
			if (CullingGrid.Instance != null)
			{
				r.enabled = true;
				CullingGrid.Instance.UnregisterInternal(r, token);
			}
		}

		private int RegisterInternal(Renderer r)
		{
			Vector3 position = r.transform.position;
			int num = Mathf.Clamp(this.WorldToGridX(position.x), 0, this._gridSize - 1);
			int num2 = Mathf.Clamp(this.WorldToGridY(position.z), 0, this._gridSize - 1);
			CullingGrid.Cell cell = this._rendererGrid[num, num2];
			cell._renderers.Add(r);
			r.enabled = cell._enabled;
			return num * this._gridSize + num2;
		}

		private void UnregisterInternal(Renderer r, int token)
		{
			int num = token / this._gridSize;
			int num2 = token - num * this._gridSize;
			this._rendererGrid[num, num2]._renderers.Remove(r);
		}

		private int WorldToGridX(float xPosition)
		{
			return Mathf.FloorToInt((xPosition - this._offset.x) / (float)this._gridWorldSize);
		}

		private int WorldToGridY(float zPosition)
		{
			return Mathf.FloorToInt((zPosition - this._offset.z) / (float)this._gridWorldSize);
		}

		private int WorldToGridXRounded(float xPosition)
		{
			return Mathf.RoundToInt((xPosition - this._offset.x) / (float)this._gridWorldSize);
		}

		private int WorldToGridYRounded(float zPosition)
		{
			return Mathf.RoundToInt((zPosition - this._offset.z) / (float)this._gridWorldSize);
		}

		private float GridToWorldX(float xPosition)
		{
			return xPosition * (float)this._gridWorldSize + this._offset.x;
		}

		private float GridToWorldZ(float yPosition)
		{
			return yPosition * (float)this._gridWorldSize + this._offset.z;
		}

		private float GetFinalRadius()
		{
			return this._circleRadius;
		}
	}
}
