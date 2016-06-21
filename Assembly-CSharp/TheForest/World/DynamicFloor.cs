using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.World
{
	public class DynamicFloor : MonoBehaviour
	{
		private Vector3 _prevPlayerPosition;

		private Vector3 _prevLocalPlayerOffset;

		private Rigidbody _rb;

		private float _extents;

		public bool houseBoat;

		private void Awake()
		{
			base.enabled = false;
			this._rb = base.GetComponent<Rigidbody>();
			this._extents = base.GetComponent<Collider>().bounds.extents.magnitude;
		}

		private void LateUpdate()
		{
			float num = this._extents;
			if (this.houseBoat)
			{
				num += 4f;
			}
			if (Vector3.Distance(LocalPlayer.Transform.position, base.transform.position) * 1.1f > num || LocalPlayer.AnimControl.oarHeld.activeSelf)
			{
				base.enabled = false;
				if (this._rb)
				{
					this._rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
				}
			}
			else
			{
				Vector3 b = LocalPlayer.Transform.position - this._prevPlayerPosition;
				LocalPlayer.Transform.position = base.transform.position + base.transform.TransformDirection(this._prevLocalPlayerOffset) + b;
				this._prevPlayerPosition = LocalPlayer.Transform.position;
				this._prevLocalPlayerOffset = base.transform.InverseTransformDirection(LocalPlayer.Transform.position - base.transform.position);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!base.enabled && other.name.Equals("UnderfootCollider"))
			{
				base.enabled = true;
				this._prevPlayerPosition = LocalPlayer.Transform.position;
				this._prevLocalPlayerOffset = base.transform.InverseTransformDirection(LocalPlayer.Transform.position - base.transform.position);
				if (this._rb)
				{
					this._rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (base.enabled && other.name.Equals("UnderfootCollider"))
			{
				base.enabled = false;
				if (this._rb)
				{
					this._rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
				}
			}
		}

		private void StoreCurrent()
		{
		}
	}
}
