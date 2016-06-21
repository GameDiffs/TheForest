using System;
using UnityEngine;

namespace TheForest.Buildings.World
{
	public class RaftPushMP : MonoBehaviour
	{
		public float _commandDuration = 0.35f;

		public RaftPush _raft;

		private RaftPush.MoveDirection _direction;

		private float _rotation;

		private float _activeCommandTime;

		private void Awake()
		{
			base.enabled = false;
			if (BoltNetwork.isClient)
			{
				base.GetComponent<Buoyancy>().enabled = false;
				UnityEngine.Object.Destroy(base.GetComponent<raftOnLand>());
				UnityEngine.Object.Destroy(base.GetComponent<Rigidbody>());
				UnityEngine.Object.Destroy(this);
			}
		}

		private void Update()
		{
			if (this._activeCommandTime > Time.realtimeSinceStartup)
			{
				this._raft.PushRaft(this._direction);
				this._raft.TurnRaft(this._rotation);
			}
			else
			{
				base.enabled = false;
			}
		}

		public void ReceivedCommand(RaftPush.MoveDirection direction, float rotation)
		{
			this._direction = direction;
			this._rotation = rotation;
			this._activeCommandTime = Time.realtimeSinceStartup + this._commandDuration;
			base.enabled = true;
		}
	}
}
