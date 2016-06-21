using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.World
{
	[DoNotSerializePublic, RequireComponent(typeof(MeshFilter))]
	public class CaveMapDrawer : MonoBehaviour
	{
		public Color _fromColor = new Color(0f, 0f, 0f, 0f);

		public Color _toColor = new Color(1f, 1f, 1f, 1f);

		public Material _caveMap;

		public Material _caveMapBG;

		public Material _worldMap;

		public Material _worldMapBG;

		public MeshFilter _targetMeshFilter;

		public Renderer _rendererBG;

		public Transform _playerPositionPin;

		public GameObject _compass;

		public int _gridSize = 10;

		public float _gridCellWorldSize = 350f;

		public float _gridCellOffset = -1750f;

		public float _mapXmin = 1443.252f;

		public float _mapXmax = -1305.252f;

		public float _mapZmin = -1178.557f;

		public float _mapZmax = 1470.557f;

		public float _worldMapXmin = 1750f;

		public float _worldMapXmax = -1750f;

		public float _worldMapZmin = -1750f;

		public float _worldMapZmax = 1750f;

		[ItemIdPicker]
		public int _caveMapItemId;

		[ItemIdPicker]
		public int _compassItemId;

		[Header("Gizmos")]
		public bool _showVisitedAreaGrid;

		public int _textX;

		public int _textY;

		public bool _test;

		public bool _clear;

		[SerializeThis]
		private bool[,] _visitedAreas;

		[SerializeThis]
		private bool[,] _worldVisitedAreas;

		private Renderer _renderer;

		private Mesh _mesh;

		private float _pinAlpha;

		private bool _togglingMaterial;

		private Color[] _colors;

		private void Awake()
		{
			this._renderer = this._targetMeshFilter.GetComponent<Renderer>();
			this.Clear();
			base.enabled = false;
			if (!LevelSerializer.IsDeserializing)
			{
				base.InvokeRepeating("CheckMapOwnerchip", 1f, 1f);
				base.InvokeRepeating("CheckMaterialToggle", 1f, 2f);
			}
		}

		private void Update()
		{
			float num = Mathf.Abs(this._mapXmin - this._mapXmax);
			float num2 = Mathf.Abs(this._mapZmin - this._mapZmax);
			float t = Mathf.Abs(LocalPlayer.Transform.position.x - this._mapXmin) / num;
			float t2 = Mathf.Abs(LocalPlayer.Transform.position.z - this._mapZmin) / num2;
			float num3 = Mathf.Lerp(0f, (float)(this._gridSize - 1), t);
			float num4 = Mathf.Lerp(0f, (float)(this._gridSize - 1), t2);
			int num5 = Mathf.RoundToInt(Mathf.Lerp(0f, (float)(this._gridSize - 1), t));
			int num6 = Mathf.RoundToInt(Mathf.Lerp(0f, (float)(this._gridSize - 1), t2));
			int num7 = (Clock.InCave || num3 - (float)((int)num3) >= 0.25f || num5 <= 1) ? 0 : -1;
			int num8 = (Clock.InCave || num4 - (float)((int)num4) >= 0.25f || num6 <= 1) ? 0 : -1;
			int num9 = (Clock.InCave || num3 - (float)((int)num3) <= 0.75f) ? 0 : ((int)Mathf.Clamp01((float)(this._gridSize - num5 - 1)));
			int num10 = (Clock.InCave || num4 - (float)((int)num4) <= 0.75f) ? 0 : ((int)Mathf.Clamp01((float)(this._gridSize - num6 - 1)));
			for (int i = (num5 <= 0) ? 0 : num7; i <= num9; i++)
			{
				for (int j = (num6 <= 0) ? 0 : num8; j <= num10; j++)
				{
					if (Clock.InCave)
					{
						if (!this._visitedAreas[num6 + j, num5 + i])
						{
							this._visitedAreas[num6 + j, num5 + i] = true;
							this.ToggleMeshAt(this._gridSize - num5 + i, num6 + j);
						}
					}
					else if (!this._worldVisitedAreas[num6 + j, num5 + i])
					{
						this._worldVisitedAreas[num6 + j, num5 + i] = true;
						this.ToggleMeshAt(this._gridSize - num5 + i, num6 + j);
					}
				}
			}
			Vector3 localPos = new Vector3(Mathf.Lerp(0.5f, -0.5f, t), Mathf.Lerp(-0.5f, 0.5f, t2), 0f);
			this.PlantPlayerPositionPin(localPos);
		}

		[DebuggerHidden]
		private IEnumerator OnDeserialized()
		{
			CaveMapDrawer.<OnDeserialized>c__Iterator170 <OnDeserialized>c__Iterator = new CaveMapDrawer.<OnDeserialized>c__Iterator170();
			<OnDeserialized>c__Iterator.<>f__this = this;
			return <OnDeserialized>c__Iterator;
		}

		private void Reload()
		{
			if (this._visitedAreas != null)
			{
				for (int i = 0; i < this._gridSize; i++)
				{
					for (int j = 0; j < this._gridSize; j++)
					{
						if ((!Clock.InCave) ? this._worldVisitedAreas[j, i] : this._visitedAreas[j, i])
						{
							this.ToggleMeshAt(this._gridSize - i, j);
						}
					}
				}
			}
		}

		private void CheckMapOwnerchip()
		{
			if (!base.enabled && LocalPlayer.Inventory.Owns(this._caveMapItemId))
			{
				base.enabled = true;
				if (this._visitedAreas == null)
				{
					this._visitedAreas = new bool[this._gridSize, this._gridSize];
					this._worldVisitedAreas = new bool[this._gridSize, this._gridSize];
					float num = Mathf.Abs(this._mapXmin - this._mapXmax);
					float num2 = Mathf.Abs(this._mapZmin - this._mapZmax);
					float t = Mathf.Abs(LocalPlayer.Transform.position.x - this._mapXmin) / num;
					float t2 = Mathf.Abs(LocalPlayer.Transform.position.z - this._mapZmin) / num2;
					float num3 = Mathf.Lerp(0f, (float)(this._gridSize - 1), t);
					float num4 = Mathf.Lerp(0f, (float)(this._gridSize - 1), t2);
					int num5 = Mathf.RoundToInt(num3);
					int num6 = Mathf.RoundToInt(num4);
					int num7 = (num3 - (float)((int)num3) >= 0.5f) ? 0 : ((int)Mathf.Clamp01((float)(num5 - 1)));
					int num8 = (num4 - (float)((int)num4) >= 0.5f) ? 0 : ((int)Mathf.Clamp01((float)(num6 - 1)));
					int num9 = (num3 - (float)((int)num3) <= 0.5f) ? 0 : ((int)Mathf.Clamp01((float)(this._gridSize - num5 - 1)));
					int num10 = (num4 - (float)((int)num4) <= 0.5f) ? 0 : ((int)Mathf.Clamp01((float)(this._gridSize - num6 - 1)));
					for (int i = (num5 <= 0) ? 0 : num7; i <= num9; i++)
					{
						for (int j = (num6 <= 0) ? 0 : num8; j <= num10; j++)
						{
							if (!this._visitedAreas[num6 + j, num5 + i])
							{
								this._visitedAreas[num6 + j, num5 + i] = true;
								this.ToggleMeshAt(this._gridSize - num5 + i, num6 + j);
							}
						}
					}
				}
				base.CancelInvoke("CheckMapOwnerchip");
			}
		}

		private void CheckCompassOwnerchip()
		{
			bool flag = !LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.LeftHand, this._compassItemId) && LocalPlayer.Inventory.Owns(this._compassItemId);
			if (flag != this._compass.activeSelf)
			{
				this._compass.SetActive(flag);
			}
		}

		private void CheckMaterialToggle()
		{
			if (!this._togglingMaterial && this._renderer.sharedMaterial.Equals(this._caveMap) != Clock.InCave)
			{
				if (this._targetMeshFilter.gameObject.activeSelf)
				{
					LocalPlayer.Inventory.MemorizeItem(Item.EquipmentSlot.RightHand);
					LocalPlayer.Inventory.StashEquipedWeapon(true);
					this._togglingMaterial = true;
					base.Invoke("MaterialToggle", 0.5f);
				}
				else
				{
					this.MaterialToggle();
				}
			}
		}

		private void MaterialToggle()
		{
			this._togglingMaterial = false;
			this._renderer.sharedMaterial = ((!Clock.InCave) ? this._worldMap : this._caveMap);
			this._rendererBG.sharedMaterial = ((!Clock.InCave) ? this._worldMapBG : this._caveMapBG);
			this.Clear();
			this.Reload();
		}

		private void CheckMesh()
		{
			if (!this._mesh || this._mesh != this._targetMeshFilter.mesh)
			{
				this._targetMeshFilter.mesh = base.GetComponent<MeshFilter>().sharedMesh;
				Mesh expr_4C = this._targetMeshFilter.mesh;
				expr_4C.name += UnityEngine.Random.Range(0, 100000).ToString();
				this._mesh = this._targetMeshFilter.mesh;
			}
		}

		private int WorldToGridRounded(float worldPosition)
		{
			return Mathf.FloorToInt((worldPosition - this._gridCellOffset) / this._gridCellWorldSize);
		}

		private void ToggleMeshAt(int x, int y)
		{
			this.CheckMesh();
			base.StartCoroutine(this.RevealMapRoutine(x * (this._gridSize + 1) + y));
		}

		[DebuggerHidden]
		private IEnumerator RevealMapRoutine(int index)
		{
			CaveMapDrawer.<RevealMapRoutine>c__Iterator171 <RevealMapRoutine>c__Iterator = new CaveMapDrawer.<RevealMapRoutine>c__Iterator171();
			<RevealMapRoutine>c__Iterator.index = index;
			<RevealMapRoutine>c__Iterator.<$>index = index;
			<RevealMapRoutine>c__Iterator.<>f__this = this;
			return <RevealMapRoutine>c__Iterator;
		}

		private void Clear()
		{
			this.CheckMesh();
			if (this._mesh.colors.Length != this._mesh.vertices.Length)
			{
				this._colors = new Color[this._mesh.vertices.Length];
			}
			else
			{
				this._colors = this._mesh.colors;
			}
			for (int i = 0; i < this._colors.Length; i++)
			{
				this._colors[i] = new Color(0f, 0f, 0f, 0f);
			}
			this._mesh.colors = this._colors;
		}

		private void PlantPlayerPositionPin(Vector3 localPos)
		{
			if (this._targetMeshFilter.gameObject.activeSelf)
			{
				if (!this._playerPositionPin.gameObject.activeSelf)
				{
					this._playerPositionPin.gameObject.SetActive(true);
				}
			}
			else if (this._playerPositionPin.gameObject.activeSelf)
			{
				this._playerPositionPin.gameObject.SetActive(false);
			}
			if (this._playerPositionPin.gameObject.activeSelf)
			{
				this._playerPositionPin.localPosition = localPos;
			}
		}

		[DebuggerHidden]
		private IEnumerator PlayerPositionPinAnim(Vector3 targetPos)
		{
			CaveMapDrawer.<PlayerPositionPinAnim>c__Iterator172 <PlayerPositionPinAnim>c__Iterator = new CaveMapDrawer.<PlayerPositionPinAnim>c__Iterator172();
			<PlayerPositionPinAnim>c__Iterator.targetPos = targetPos;
			<PlayerPositionPinAnim>c__Iterator.<$>targetPos = targetPos;
			<PlayerPositionPinAnim>c__Iterator.<>f__this = this;
			return <PlayerPositionPinAnim>c__Iterator;
		}
	}
}
