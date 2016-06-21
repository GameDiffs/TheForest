using System;
using TheForest.Buildings.Interfaces;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Bridge Anchor")]
	public class StructureAnchor : MonoBehaviour
	{
		public GameObject _icon;

		[SerializeThis]
		public IAnchorableStructure _hookedInStructure;

		[SerializeThis, Range(1f, 15f)]
		public int _anchorNum = 1;

		private void Start()
		{
			if (this._icon)
			{
				this._icon.SetActive(false);
			}
			base.enabled = false;
		}

		private void Update()
		{
			if (!LocalPlayer.Create.CreateMode)
			{
				if (this._icon)
				{
					this._icon.SetActive(false);
				}
				base.enabled = false;
			}
		}

		private void OnDestroy()
		{
			if (this._hookedInStructure != null)
			{
				this._hookedInStructure.AnchorDestroyed(this);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("ShowAnchorArea") && this._hookedInStructure == null)
			{
				if (this._icon)
				{
					this._icon.SetActive(true);
				}
				base.enabled = true;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("ShowAnchorArea"))
			{
				if (this._icon)
				{
					this._icon.SetActive(false);
				}
				base.enabled = false;
			}
		}

		public long ToHash()
		{
			return (long)(Mathf.RoundToInt(base.transform.position.x * 10f) + Mathf.RoundToInt(base.transform.position.y * 10f) * 1000000) + (long)Mathf.RoundToInt(base.transform.position.y * 10f) * 1000000000000L;
		}
	}
}
