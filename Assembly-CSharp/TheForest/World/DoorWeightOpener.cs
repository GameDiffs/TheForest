using System;
using TheForest.Buildings.World;
using UnityEngine;

namespace TheForest.World
{
	[DoNotSerializePublic]
	public class DoorWeightOpener : MonoBehaviour
	{
		public Transform _doorTr;

		public Transform _closedDoorRotationTr;

		public Transform _openedDoorRotationTr;

		public WeaponRackSlot[] _slots;

		[Space(10f)]
		public Transform _doorFlippedTr;

		public Transform _closedDoorFlippedRotationTr;

		public Transform _openedDoorFlippedRotationTr;

		[Space(10f)]
		public Transform _weightTr;

		public Transform _closedWeightPositionTr;

		public Transform _openedWeightPositionTr;

		[Space(10f)]
		public int _maxWeight = 5;

		public float _openSmoothTime = 1f;

		[Header("FMOD")]
		public Transform _sfxPosition;

		public string _initialOpenEvent;

		public string _additionalOpenEvent;

		public string _beginCloseEvent;

		public string _fullyClosedEvent;

		public float _fullyClosedThreshold = 0.15f;

		public int _weight;

		private Vector3 _targetDoorRotation;

		private Vector3 _targetDoorFlippedRotation;

		private float _doorVelocity;

		private float _doorFlippedVelocity;

		private Vector3 _targetWeightPosition;

		private Vector3 _weightVelocity = Vector3.zero;

		private void Start()
		{
			this._targetDoorRotation = this._closedDoorRotationTr.eulerAngles;
			this._targetDoorFlippedRotation = this._closedDoorFlippedRotationTr.eulerAngles;
			this._targetWeightPosition = this._closedWeightPositionTr.position;
		}

		private void Update()
		{
			bool flag = this.IsClosed();
			Vector3 eulerAngles = this._doorTr.eulerAngles;
			eulerAngles.y = Mathf.SmoothDampAngle(this._doorTr.eulerAngles.y, this._targetDoorRotation.y, ref this._doorVelocity, this._openSmoothTime);
			this._doorTr.eulerAngles = eulerAngles;
			Vector3 eulerAngles2 = this._doorFlippedTr.eulerAngles;
			eulerAngles2.y = Mathf.SmoothDampAngle(this._doorFlippedTr.eulerAngles.y, this._targetDoorFlippedRotation.y, ref this._doorFlippedVelocity, this._openSmoothTime);
			this._doorFlippedTr.eulerAngles = eulerAngles2;
			this._weightTr.position = Vector3.SmoothDamp(this._weightTr.position, this._targetWeightPosition, ref this._weightVelocity, this._openSmoothTime);
			if (this.IsClosed() && !flag)
			{
				FMODCommon.PlayOneshot(this._fullyClosedEvent, this._sfxPosition);
			}
		}

		private void OnEnable()
		{
			FMODCommon.PreloadEvents(this.AllEventPaths());
		}

		private void OnDisable()
		{
			FMODCommon.UnloadEvents(this.AllEventPaths());
		}

		public void OnWeightAdded()
		{
			this.PlayOpenEvent();
			this._weight++;
			this.RefreshDoorAndWeightTarget();
			base.SendMessageUpwards("OnWeightChanged", SendMessageOptions.DontRequireReceiver);
		}

		public void OnWeightRemoved()
		{
			this._weight--;
			this.RefreshDoorAndWeightTarget();
			if (this._weight < this._maxWeight)
			{
				FMODCommon.PlayOneshot(this._beginCloseEvent, this._sfxPosition);
			}
			base.SendMessageUpwards("OnWeightChanged", SendMessageOptions.DontRequireReceiver);
		}

		public void OpenFull()
		{
			this.PlayOpenEvent();
			this._weight += this._maxWeight;
			this.RefreshDoorAndWeightTarget();
		}

		private void RefreshDoorAndWeightTarget()
		{
			this._targetDoorRotation = Quaternion.Lerp(this._closedDoorRotationTr.rotation, this._openedDoorRotationTr.rotation, (float)this._weight / (float)this._maxWeight).eulerAngles;
			this._targetDoorFlippedRotation = Quaternion.Lerp(this._closedDoorFlippedRotationTr.rotation, this._openedDoorFlippedRotationTr.rotation, (float)this._weight / (float)this._maxWeight).eulerAngles;
			this._targetWeightPosition = Vector3.Lerp(this._closedWeightPositionTr.position, this._openedWeightPositionTr.position, (float)this._weight / (float)this._maxWeight);
		}

		private bool IsClosed()
		{
			return Mathf.Abs(this._doorTr.eulerAngles.y - this._closedDoorRotationTr.eulerAngles.y) < this._fullyClosedThreshold;
		}

		private void PlayOpenEvent()
		{
			if (this._weight == 0)
			{
				FMODCommon.PlayOneshot(this._initialOpenEvent, this._sfxPosition);
			}
			else if (this._weight < this._maxWeight)
			{
				FMODCommon.PlayOneshot(this._additionalOpenEvent, this._sfxPosition);
			}
		}

		private string[] AllEventPaths()
		{
			return new string[]
			{
				this._initialOpenEvent,
				this._additionalOpenEvent,
				this._beginCloseEvent,
				this._fullyClosedEvent
			};
		}
	}
}
