using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic]
	public class CraneTrigger : MonoBehaviour
	{
		private enum States
		{
			Idle,
			Locked
		}

		public float _onRopeOffset;

		public float _ropeAttachOffset;

		public float _ropeAttachTopOffsetZ;

		public float _ropeAttachTopOffsetY;

		public float _ropeAttachTopOffsetX;

		public float _maxHeight = 50f;

		public float _moveSpeed = 1f;

		public Transform _platformTr;

		public Transform[] _ropes;

		public Transform[] _ropeTargets;

		public GameObject _iconSheen;

		public GameObject _iconPickUp;

		private CraneTrigger.States _state;

		private int _climbHash;

		private int _climbIdleHash;

		private void Awake()
		{
			base.enabled = false;
			this._state = CraneTrigger.States.Idle;
			this._climbHash = Animator.StringToHash("climbing");
			this._climbIdleHash = Animator.StringToHash("climbIdle");
		}

		private void Update()
		{
			if (LocalPlayer.FpCharacter.PushingSled)
			{
				return;
			}
			if (TheForest.Utils.Input.GetButtonDown("Take"))
			{
				this._state = ((this._state != CraneTrigger.States.Idle) ? CraneTrigger.States.Idle : CraneTrigger.States.Locked);
			}
			else if (this._state == CraneTrigger.States.Locked)
			{
				float axis = TheForest.Utils.Input.GetAxis("Vertical");
				if (!Mathf.Approximately(axis, 0f))
				{
					if (axis < 0f)
					{
						this._platformTr.localPosition += new Vector3(0f, -this._moveSpeed * Time.deltaTime, 0f);
						if (this._platformTr.localPosition.y < -this._maxHeight)
						{
							this._platformTr.localPosition = new Vector3(this._platformTr.localPosition.x, -this._maxHeight, this._platformTr.localPosition.z);
						}
					}
					else
					{
						this._platformTr.localPosition += new Vector3(0f, this._moveSpeed * Time.deltaTime, 0f);
						if (this._platformTr.localPosition.y > 0f)
						{
							this._platformTr.localPosition = new Vector3(this._platformTr.localPosition.x, 0f, this._platformTr.localPosition.z);
						}
					}
					for (int i = 0; i < this._ropes.Length; i++)
					{
						this._ropes[i].LookAt(this._ropeTargets[i]);
					}
				}
			}
		}

		private void GrabEnter()
		{
			base.enabled = true;
			this._iconSheen.SetActive(false);
			this._iconPickUp.SetActive(true);
		}

		private void GrabExit()
		{
			if (base.enabled)
			{
				base.enabled = true;
				this._iconSheen.SetActive(true);
				this._iconPickUp.SetActive(false);
			}
		}

		private void LockPlayer()
		{
		}

		private void UnlockPlayer()
		{
		}

		private void StickToRope()
		{
			if (LocalPlayer.Animator.GetCurrentAnimatorStateInfo(0).tagHash == this._climbHash || LocalPlayer.Animator.GetCurrentAnimatorStateInfo(0).tagHash == this._climbIdleHash)
			{
				Vector3 position = base.transform.position - base.transform.forward * this._onRopeOffset;
				position.y = LocalPlayer.Transform.position.y;
				LocalPlayer.Transform.position = position;
				LocalPlayer.Transform.rotation = base.transform.rotation;
			}
		}
	}
}
