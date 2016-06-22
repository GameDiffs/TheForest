using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Anchorable Structure")]
	public class SingleAnchorStructure : MonoBehaviour
	{
		public LayerMask _floorLayers;

		public float _minHeight = 10f;

		[SerializeThis]
		public bool _wasPlaced;

		public bool _wasBuilt;

		public bool _destroyWhenPlaced;

		public Renderer _renderer;

		[SerializeThis]
		private StructureAnchor _anchor1;

		public StructureAnchor Anchor1
		{
			get
			{
				return this._anchor1;
			}
			set
			{
				this._anchor1 = value;
			}
		}

		private void Awake()
		{
			base.StartCoroutine(this.DelayedAwake(LevelSerializer.IsDeserializing));
			base.enabled = false;
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake(bool isDeserializing)
		{
			SingleAnchorStructure.<DelayedAwake>c__Iterator141 <DelayedAwake>c__Iterator = new SingleAnchorStructure.<DelayedAwake>c__Iterator141();
			<DelayedAwake>c__Iterator.<>f__this = this;
			return <DelayedAwake>c__Iterator;
		}

		private void Update()
		{
			if (!base.transform.parent && Vector3.Distance(LocalPlayer.Create.BuildingPlacer.transform.position, base.transform.position) > 3.75f)
			{
				base.transform.parent = LocalPlayer.Create.BuildingPlacer.transform;
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				if (this._anchor1._hookedInStructure == this)
				{
					this._anchor1._hookedInStructure = null;
				}
				this._anchor1 = null;
			}
			bool flag = this._anchor1;
			if (LocalPlayer.Create.BuildingPlacer.Clear != flag || Scene.HudGui.PlaceWallIcon.activeSelf != flag)
			{
				Scene.HudGui.PlaceWallIcon.SetActive(flag);
				LocalPlayer.Create.BuildingPlacer.Clear = flag;
			}
			if (this._renderer)
			{
				this._renderer.sharedMaterial = ((!flag) ? LocalPlayer.Create.BuildingPlacer.RedMat : LocalPlayer.Create.BuildingPlacer.ClearMat);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (base.enabled)
			{
				StructureAnchor component = other.GetComponent<StructureAnchor>();
				if (component && component != this._anchor1 && component._hookedInStructure == null)
				{
					RaycastHit raycastHit;
					if (!Physics.Raycast(component.transform.position + component.transform.forward, Vector3.down, out raycastHit, this._minHeight, this._floorLayers))
					{
						base.transform.parent = null;
						base.transform.position = other.transform.position;
						base.transform.rotation = other.transform.rotation;
						this._anchor1 = component;
					}
					UnityEngine.Debug.Log(raycastHit);
				}
			}
		}

		private void OnDestroy()
		{
			if (this._anchor1 && this._anchor1._hookedInStructure == this)
			{
				this._anchor1._hookedInStructure = null;
			}
		}

		private void OnDeserialized()
		{
			this.OnPlaced();
		}

		private void OnPlaced()
		{
			if (this._destroyWhenPlaced)
			{
				UnityEngine.Object.Destroy(this);
			}
			else
			{
				this._wasPlaced = true;
				base.GetComponent<Collider>().enabled = false;
				base.enabled = false;
			}
		}

		public void AnchorDestroyed(StructureAnchor anchor)
		{
			if (base.gameObject)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
