using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	public class FloorHoleArchitect : MonoBehaviour
	{
		public Vector2 _holeSize;

		public Material _offMaterial;

		public Material _onMaterial;

		public FloorArchitect _previewFloor;

		public RoofArchitect _previewRoof;

		private IList<IHoleStructure> _targets = new List<IHoleStructure>();

		private IList<IHoleStructure> _previews = new List<IHoleStructure>();

		private IList<Hole> _holes = new List<Hole>();

		private void Awake()
		{
			base.enabled = false;
		}

		private void Update()
		{
			if (this._previews.Count > 0)
			{
				for (int i = this._previews.Count - 1; i >= 0; i--)
				{
					this._holes[i]._size = this._holeSize;
					this._holes[i]._yRotation = base.transform.rotation.eulerAngles.y;
					this._holes[i]._position = base.transform.position;
					IHoleStructure holeStructure = this._previews[i];
					if (!((MonoBehaviour)holeStructure).enabled)
					{
						holeStructure.CreateStructure(false);
						if (!this._holes[i]._used)
						{
							UnityEngine.Object.Destroy((this._previews[i] as MonoBehaviour).gameObject);
							this._holes[i] = null;
							this._holes.RemoveAt(i);
							this._previews.RemoveAt(i);
							this._targets.RemoveAt(i);
							base.enabled = false;
						}
					}
				}
				if (this._previews.Count == 0)
				{
					base.GetComponent<Renderer>().sharedMaterial = this._offMaterial;
					if (LocalPlayer.Create.BuildingPlacer)
					{
						LocalPlayer.Create.BuildingPlacer.Clear = false;
					}
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			PrefabIdentifier componentInParent = other.GetComponentInParent<PrefabIdentifier>();
			if (componentInParent)
			{
				IHoleStructure holeStructure = (IHoleStructure)componentInParent.GetComponent(typeof(IHoleStructure));
				if (holeStructure != null && !this._targets.Contains(holeStructure))
				{
					Hole item;
					if (holeStructure is FloorArchitect)
					{
						FloorArchitect floorArchitect = UnityEngine.Object.Instantiate<FloorArchitect>(this._previewFloor);
						(holeStructure as FloorArchitect).OnBuilt(floorArchitect.gameObject);
						floorArchitect._wasBuilt = false;
						this._previews.Add(floorArchitect);
						item = floorArchitect.AddSquareHole(base.transform.position, base.transform.rotation.y, this._holeSize);
					}
					else
					{
						if (!(holeStructure is RoofArchitect))
						{
							Debug.LogError("Trying to cut IHS '" + componentInParent.name + "' which isn't roof or floor. Please report this to guillaume.");
							return;
						}
						RoofArchitect roofArchitect = UnityEngine.Object.Instantiate<RoofArchitect>(this._previewRoof);
						(holeStructure as RoofArchitect).OnBuilt(roofArchitect.gameObject);
						roofArchitect._wasBuilt = false;
						this._previews.Add(roofArchitect);
						item = roofArchitect.AddSquareHole(base.transform.position, base.transform.rotation.y, this._holeSize);
					}
					this._targets.Add(holeStructure);
					this._holes.Add(item);
					base.enabled = true;
					base.GetComponent<Renderer>().sharedMaterial = this._onMaterial;
					if (LocalPlayer.Create.BuildingPlacer)
					{
						LocalPlayer.Create.BuildingPlacer.Clear = true;
					}
				}
			}
		}

		private void OnDestroy()
		{
			if (this._targets.Count > 0)
			{
				for (int i = this._targets.Count - 1; i >= 0; i--)
				{
					this._targets[i].RemoveHole(this._holes[i]);
					this._holes[i] = null;
					this._holes.RemoveAt(i);
					if (!((MonoBehaviour)this._targets[i]).enabled)
					{
						this._targets[i].CreateStructure(false);
					}
					this._targets.RemoveAt(i);
					UnityEngine.Object.Destroy((this._previews[i] as MonoBehaviour).gameObject);
					this._previews.RemoveAt(i);
				}
			}
		}

		private void OnPlaced()
		{
			for (int i = 0; i < this._targets.Count; i++)
			{
				this._targets[i].AddSquareHole(this._holes[i]._position, this._holes[i]._yRotation, this._holes[i]._size);
				this._targets[i].CreateStructure(false);
				UnityEngine.Object.Destroy((this._previews[i] as MonoBehaviour).gameObject);
				this._previews[i] = null;
			}
			if (this._targets.Count > 0)
			{
				LocalPlayer.Sfx.PlayStructureBreak(base.gameObject, 0.1f);
				Vector3 vector = (base.transform.position + new Vector3(this._holeSize.x, 0f, this._holeSize.y)).RotateY(this._holes[0]._yRotation);
				Prefabs.Instance.SpawnWoodHitPS(vector, Quaternion.LookRotation(base.transform.position - vector));
				vector = (base.transform.position + new Vector3(this._holeSize.x, 0f, -this._holeSize.y)).RotateY(this._holes[0]._yRotation);
				Prefabs.Instance.SpawnWoodHitPS(vector, Quaternion.LookRotation(base.transform.position - vector));
				vector = (base.transform.position + new Vector3(-this._holeSize.x, 0f, -this._holeSize.y)).RotateY(this._holes[0]._yRotation);
				Prefabs.Instance.SpawnWoodHitPS(vector, Quaternion.LookRotation(base.transform.position - vector));
				vector = (base.transform.position + new Vector3(-this._holeSize.x, 0f, this._holeSize.y)).RotateY(this._holes[0]._yRotation);
				Prefabs.Instance.SpawnWoodHitPS(vector, Quaternion.LookRotation(base.transform.position - vector));
				this._holes.Clear();
				this._targets.Clear();
				this._previews.Clear();
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
