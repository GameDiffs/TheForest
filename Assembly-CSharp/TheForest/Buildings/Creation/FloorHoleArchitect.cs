using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	public class FloorHoleArchitect : MonoBehaviour
	{
		public Vector2 _holeSize;

		public Material _offMaterial;

		public Material _onMaterial;

		private IHoleStructure _target;

		private Hole _hole;

		private void Awake()
		{
			base.enabled = false;
		}

		private void Update()
		{
			if (this._hole != null)
			{
				this._hole._size = this._holeSize;
				this._hole._yRotation = base.transform.rotation.eulerAngles.y;
				this._hole._position = base.transform.position;
				if (!((MonoBehaviour)this._target).enabled)
				{
					this._target.CreateStructure(false);
					if (!this._hole._used)
					{
						this._target.RemoveHole(this._hole);
						this._hole = null;
						this._target = null;
						base.enabled = false;
						base.GetComponent<Renderer>().sharedMaterial = this._offMaterial;
						if (LocalPlayer.Create.BuildingPlacer)
						{
							LocalPlayer.Create.BuildingPlacer.Clear = false;
						}
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
				if (holeStructure != null && holeStructure != this._target)
				{
					if (this._target != null)
					{
						this._target.RemoveHole(this._hole);
						this._hole = null;
						if (!((MonoBehaviour)this._target).enabled)
						{
							this._target.CreateStructure(false);
						}
					}
					this._target = holeStructure;
					this._hole = holeStructure.AddSquareHole(base.transform.position, base.transform.rotation.y, this._holeSize);
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
			if (this._hole != null && this._target != null)
			{
				this._target.RemoveHole(this._hole);
				this._hole = null;
				if (!((MonoBehaviour)this._target).enabled)
				{
					this._target.CreateStructure(false);
				}
			}
		}

		private void OnPlaced()
		{
			if (this._hole != null)
			{
				LocalPlayer.Sfx.PlayStructureBreak(base.gameObject, 0.1f);
				Vector3 vector = (base.transform.position + new Vector3(this._holeSize.x, 0f, this._holeSize.y)).RotateY(this._hole._yRotation);
				Prefabs.Instance.SpawnWoodHitPS(vector, Quaternion.LookRotation(base.transform.position - vector));
				vector = (base.transform.position + new Vector3(this._holeSize.x, 0f, -this._holeSize.y)).RotateY(this._hole._yRotation);
				Prefabs.Instance.SpawnWoodHitPS(vector, Quaternion.LookRotation(base.transform.position - vector));
				vector = (base.transform.position + new Vector3(-this._holeSize.x, 0f, -this._holeSize.y)).RotateY(this._hole._yRotation);
				Prefabs.Instance.SpawnWoodHitPS(vector, Quaternion.LookRotation(base.transform.position - vector));
				vector = (base.transform.position + new Vector3(-this._holeSize.x, 0f, this._holeSize.y)).RotateY(this._hole._yRotation);
				Prefabs.Instance.SpawnWoodHitPS(vector, Quaternion.LookRotation(base.transform.position - vector));
				this._hole = null;
				this._target = null;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
