using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.World
{
	public class ElevatorSystem : MonoBehaviour
	{
		public Rigidbody _rb;

		public Transform _upPosition;

		public Transform _downPosition;

		public float _movementSmoothTime = 2f;

		public GameObject _sheenBillboardIcon;

		public GameObject _pickupBillboardIcon;

		public bool _test;

		private bool _moving;

		private void Awake()
		{
			this.GrabExit();
		}

		private void Update()
		{
			if (!this._moving)
			{
				this._sheenBillboardIcon.SetActive(false);
				this._pickupBillboardIcon.SetActive(true);
				if (TheForest.Utils.Input.GetButtonDown("Take") || this._test)
				{
					float num = Vector3.Distance(this._rb.position, this._upPosition.position);
					float num2 = Vector3.Distance(this._rb.position, this._downPosition.position);
					base.StartCoroutine(this.Goto((num <= num2) ? this._downPosition.position : this._upPosition.position));
				}
			}
			else
			{
				this._sheenBillboardIcon.SetActive(false);
				this._pickupBillboardIcon.SetActive(false);
			}
			this._test = false;
		}

		private void GrabEnter()
		{
			base.enabled = true;
		}

		private void GrabExit()
		{
			base.enabled = false;
			this._sheenBillboardIcon.SetActive(true);
			this._pickupBillboardIcon.SetActive(false);
		}

		[DebuggerHidden]
		private IEnumerator Goto(Vector3 targetPos)
		{
			ElevatorSystem.<Goto>c__Iterator1C6 <Goto>c__Iterator1C = new ElevatorSystem.<Goto>c__Iterator1C6();
			<Goto>c__Iterator1C.targetPos = targetPos;
			<Goto>c__Iterator1C.<$>targetPos = targetPos;
			<Goto>c__Iterator1C.<>f__this = this;
			return <Goto>c__Iterator1C;
		}
	}
}
