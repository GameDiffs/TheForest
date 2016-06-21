using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[AddComponentMenu("Buildings/Creation/Wall Click")]
	public class WallClick : MonoBehaviour
	{
		public Create CreateScript;

		public Transform Target;

		private Vector3 OriginalLocalPosition;

		private Quaternion OriginalLocalRotation;

		private Transform Current;

		private bool Locking;

		private void Awake()
		{
			base.enabled = false;
		}

		private void OnDeserialized()
		{
			this.Awake();
		}

		private void OnDestroy()
		{
			this.ClosePlace();
		}

		private void Update()
		{
			if (this.Locking)
			{
				this.CreateScript.BuildingPlacer.Locked = true;
				Vector3 position = this.Target.position;
				if (this.CreateScript.CurrentBlueprint._allowInTreeAtFloorLevel)
				{
					position.y = this.Current.position.y;
				}
				this.Current.position = position;
				if (!this.CreateScript.BuildingPlacer.TreeStructure)
				{
					this.Current.rotation = this.Target.rotation;
				}
				Scene.HudGui.SnapIcon.SetActive(true);
				Scene.HudGui.PlaceIcon.SetActive(false);
			}
			else
			{
				Scene.HudGui.SnapIcon.SetActive(false);
				Scene.HudGui.PlaceIcon.SetActive(this.CreateScript.BuildingPlacer.Clear);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (base.enabled && other.transform.root != this.CreateScript.CurrentGhost.transform.root)
			{
				if ((this.CreateScript.CurrentBlueprint._isPlateform || this.CreateScript.CurrentBlueprint._isStairPiece || this.CreateScript.CurrentBlueprint._isWallPiece) && other.gameObject.CompareTag("Con"))
				{
					this.Lock();
					this.Target = other.gameObject.transform;
					this.CreateScript.BuildingPlacer.Locked = false;
				}
				if (this.CreateScript.CurrentBlueprint._allowInTree && other.gameObject.CompareTag("conTree") && other.transform.parent.GetComponent<TreeHealth>().LodTree.OnTreeCutDownTarget == null)
				{
					this.Lock();
					this.Target = other.gameObject.transform;
					this.CreateScript.BuildingPlacer.Locked = false;
					this.CreateScript.TargetTree = other.gameObject.transform;
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (base.enabled && this.Target && other.transform.root == this.Target.root)
			{
				if ((this.CreateScript.CurrentBlueprint._isPlateform || this.CreateScript.CurrentBlueprint._isStairPiece || this.CreateScript.CurrentBlueprint._isWallPiece) && other.gameObject.CompareTag("Con"))
				{
					this.RevertGhostPosition();
					this.Reset();
				}
				if (this.CreateScript.CurrentBlueprint._allowInTree && other.gameObject.CompareTag("conTree"))
				{
					this.RevertGhostPosition();
					this.Reset();
				}
			}
		}

		public void ShowPlace()
		{
			base.enabled = true;
			Scene.HudGui.PlaceIcon.SetActive(true);
			Scene.HudGui.RotateIcon.SetActive(true);
		}

		public void ClosePlace()
		{
			this.Reset();
			if (Scene.HudGui && Scene.HudGui.SnapIcon)
			{
				Scene.HudGui.SnapIcon.SetActive(false);
			}
			if (Scene.HudGui && Scene.HudGui.PlaceIcon)
			{
				Scene.HudGui.PlaceIcon.SetActive(false);
			}
			if (Scene.HudGui && Scene.HudGui.RotateIcon)
			{
				Scene.HudGui.RotateIcon.SetActive(false);
			}
			base.enabled = false;
		}

		public void Reset()
		{
			if (Scene.HudGui && Scene.HudGui.SnapIcon)
			{
				Scene.HudGui.SnapIcon.SetActive(false);
			}
			if (this.CreateScript.BuildingPlacer)
			{
				this.CreateScript.BuildingPlacer.Locked = false;
			}
			this.Locking = false;
			this.Target = null;
		}

		private void Lock()
		{
			if (!this.Locking)
			{
				this.Current = this.CreateScript.CurrentGhost.transform;
				this.OriginalLocalPosition = this.Current.localPosition;
				this.OriginalLocalRotation = this.Current.localRotation;
				this.Locking = true;
			}
		}

		private void ResetDelay()
		{
		}

		private void RevertGhostPosition()
		{
			this.Current.localPosition = this.OriginalLocalPosition;
			this.Current.localRotation = this.OriginalLocalRotation;
		}
	}
}
