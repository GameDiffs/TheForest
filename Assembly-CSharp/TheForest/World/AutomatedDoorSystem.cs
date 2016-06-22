using System;
using UnityEngine;

namespace TheForest.World
{
	public class AutomatedDoorSystem : MonoBehaviour
	{
		[Serializable]
		public class Door
		{
			public Transform _door;

			public Transform _openedPosition;

			public Transform _closedPosition;
		}

		public enum State
		{
			Closed,
			Closing,
			Opened,
			Opening
		}

		public AutomatedDoorSystem.Door[] _doors;

		public string[] _tags = new string[]
		{
			"Player"
		};

		public AutomatedDoorSystem.State _state;

		public float _duration = 0.75f;

		private float _alpha;

		private void Awake()
		{
			base.enabled = false;
			this.UpdateDoors();
		}

		private void Update()
		{
			float num = Time.deltaTime * (1f / this._duration);
			if (this._state < AutomatedDoorSystem.State.Opened)
			{
				this._alpha -= num;
				if (this._alpha <= 0f)
				{
					this._alpha = 0f;
					this._state = AutomatedDoorSystem.State.Closed;
					base.enabled = false;
				}
			}
			else
			{
				this._alpha += num;
				if (this._alpha >= 1f)
				{
					this._alpha = 1f;
					this._state = AutomatedDoorSystem.State.Opened;
					base.enabled = false;
				}
			}
			this.UpdateDoors();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (this._state < AutomatedDoorSystem.State.Opened)
			{
				string[] tags = this._tags;
				for (int i = 0; i < tags.Length; i++)
				{
					string tag = tags[i];
					if (other.CompareTag(tag))
					{
						this._state = AutomatedDoorSystem.State.Opening;
						base.enabled = true;
						break;
					}
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (this._state > AutomatedDoorSystem.State.Closing)
			{
				string[] tags = this._tags;
				for (int i = 0; i < tags.Length; i++)
				{
					string tag = tags[i];
					if (other.CompareTag(tag))
					{
						this._state = AutomatedDoorSystem.State.Closing;
						base.enabled = true;
						break;
					}
				}
			}
		}

		private void UpdateDoors()
		{
			AutomatedDoorSystem.Door[] doors = this._doors;
			for (int i = 0; i < doors.Length; i++)
			{
				AutomatedDoorSystem.Door door = doors[i];
				door._door.position = Vector3.Lerp(door._closedPosition.position, door._openedPosition.position, this._alpha);
				door._door.rotation = Quaternion.Slerp(door._closedPosition.rotation, door._openedPosition.rotation, this._alpha);
			}
		}
	}
}
