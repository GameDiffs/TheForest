using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	public class BuildingWarmthArea : MonoBehaviour
	{
		private bool _activeWarmth;

		private bool _hasPlayer;

		private void Awake()
		{
			base.enabled = false;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player") && !this._hasPlayer)
			{
				this._hasPlayer = true;
				if (this._activeWarmth)
				{
					LocalPlayer.GameObject.SendMessage("HomeWarmth");
				}
			}
			if (other.CompareTag("FireTrigger"))
			{
				Transform transform = base.GetComponentInParent<PrefabIdentifier>().transform;
				bool activeWarmth = this._activeWarmth;
				this._activeWarmth = false;
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Fire2 componentInChildren = transform.GetChild(i).GetComponentInChildren<Fire2>();
					if (componentInChildren && componentInChildren.Lit && base.GetComponent<Collider>().bounds.Contains(componentInChildren.transform.position))
					{
						this._activeWarmth = true;
						break;
					}
				}
				if (this._hasPlayer && activeWarmth != this._activeWarmth)
				{
					LocalPlayer.GameObject.SendMessage((!this._activeWarmth) ? "LeaveHomeWarmth" : "HomeWarmth");
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Player") && this._hasPlayer)
			{
				this._hasPlayer = false;
				if (this._activeWarmth)
				{
					LocalPlayer.GameObject.SendMessage("LeaveHomeWarmth");
				}
			}
			if (other.CompareTag("FireTrigger") && this._activeWarmth)
			{
				Transform transform = base.GetComponentInParent<PrefabIdentifier>().transform;
				bool activeWarmth = this._activeWarmth;
				this._activeWarmth = false;
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Fire2 componentInChildren = transform.GetChild(i).GetComponentInChildren<Fire2>();
					if (componentInChildren && componentInChildren.Lit && base.GetComponent<Collider>().bounds.Contains(componentInChildren.transform.position))
					{
						this._activeWarmth = true;
						break;
					}
				}
				if (this._hasPlayer && activeWarmth != this._activeWarmth)
				{
					LocalPlayer.GameObject.SendMessage((!this._activeWarmth) ? "LeaveHomeWarmth" : "HomeWarmth");
				}
			}
		}
	}
}
