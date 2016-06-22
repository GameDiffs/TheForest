using System;
using UnityEngine;

namespace TheForest.World
{
	public class EnableInWaterProxy : MonoBehaviour
	{
		public Behaviour _target;

		private bool _inWater;

		private void OnEnable()
		{
			if (this._inWater && this._target)
			{
				this._target.enabled = true;
			}
		}

		private void OnDisable()
		{
			if (this._target)
			{
				this._target.enabled = false;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!this._target.enabled && other.CompareTag("Water"))
			{
				this._inWater = true;
				if (base.enabled)
				{
					this._target.enabled = true;
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (this._target.enabled && other.CompareTag("Water"))
			{
				this._inWater = false;
				this._target.enabled = false;
			}
		}
	}
}
