using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Wall Addition Trigger")]
	public class WallAdditionTrigger : MonoBehaviour
	{
		public int _edgeNum;

		public int _segmentNum;

		private bool _gamepadLock;

		private void Awake()
		{
			base.enabled = false;
		}

		private void Update()
		{
			if (LocalPlayer.Inventory.Logs.HasLogs)
			{
				if (!base.GetComponent<Craft_Structure>().enabled)
				{
					base.transform.GetComponentInParent<WallChunkArchitect>().HideToggleAdditionIcon();
					base.GetComponent<Craft_Structure>().GrabEnter();
				}
			}
			else
			{
				if (base.GetComponent<Craft_Structure>().enabled)
				{
					base.transform.GetComponentInParent<WallChunkArchitect>().ShowToggleAdditionIcon();
					base.GetComponent<Craft_Structure>().GrabExit();
					Scene.HudGui.DestroyIcon.gameObject.SetActive(true);
				}
				if (TheForest.Utils.Input.IsGamePad && this._gamepadLock)
				{
					this._gamepadLock = (TheForest.Utils.Input.GetAxis("Rotate") > 0f);
				}
				else if ((!TheForest.Utils.Input.IsGamePad && TheForest.Utils.Input.GetButtonDown("Rotate")) || (TheForest.Utils.Input.IsGamePad && TheForest.Utils.Input.GetAxis("Rotate") > 0f))
				{
					this._gamepadLock = TheForest.Utils.Input.IsGamePad;
					base.transform.GetComponentInParent<WallChunkArchitect>().ToggleSegmentAddition();
					LocalPlayer.Sfx.PlayWhoosh();
				}
				if (TheForest.Utils.Input.GetButtonAfterDelay("Craft", 0.5f))
				{
					base.GetComponent<Craft_Structure>().CancelBlueprint();
					base.transform.GetComponentInParent<WallChunkArchitect>().HideToggleAdditionIcon();
					return;
				}
			}
		}

		private void OnDestroy()
		{
			if (base.enabled)
			{
				WallChunkArchitect componentInParent = base.transform.GetComponentInParent<WallChunkArchitect>();
				if (componentInParent)
				{
					componentInParent.HideToggleAdditionIcon();
				}
			}
		}

		private void GrabEnter()
		{
			base.enabled = true;
		}

		private void GrabExit()
		{
			if (base.enabled)
			{
				base.transform.GetComponentInParent<WallChunkArchitect>().HideToggleAdditionIcon();
				base.enabled = false;
			}
		}
	}
}
