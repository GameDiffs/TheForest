using Bolt;
using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

public class CoopPlayerRaft : EntityBehaviour<IPlayerState>
{
	private BoltEntity _raftEntity;

	private Transform _raftTransform;

	private bool _hasRaftEntity;

	private List<Collider> _boatMpTriggers = new List<Collider>();

	private void Awake()
	{
		base.enabled = false;
	}

	public override void Attached()
	{
		this._raftTransform = new GameObject("RaftTransform").transform;
		base.state.RaftTransform.SetTransforms(this._raftTransform);
		base.state.AddCallback("RaftEntity", new PropertyCallbackSimple(this.OnRaftEntityChange));
		base.enabled = true;
	}

	private void OnRaftEntityChange()
	{
		this._raftEntity = base.state.RaftEntity;
		this._hasRaftEntity = this._raftEntity;
	}

	public override void Detached()
	{
		if (this._raftTransform)
		{
			UnityEngine.Object.Destroy(this._raftTransform.gameObject);
			this._raftTransform = null;
		}
		this._raftEntity = null;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (BoltNetwork.isRunning && this.entity.IsOwner() && other.gameObject.CompareTag("RaftMPTrigger"))
		{
			if (!this._boatMpTriggers.Contains(other))
			{
				this._boatMpTriggers.Add(other);
			}
			base.state.RaftEntity = other.gameObject.GetComponentInParent<BoltEntity>();
		}
	}

	private void LateUpdate()
	{
		if (this.entity.isAttached)
		{
			if (this.entity.isOwner)
			{
				if (this._boatMpTriggers.Count > 0)
				{
					for (int i = this._boatMpTriggers.Count - 1; i >= 0; i--)
					{
						if (this._boatMpTriggers[i] == null || !this._boatMpTriggers[i].bounds.Intersects(LocalPlayer.FpCharacter.capsule.bounds))
						{
							this._boatMpTriggers.RemoveAt(i);
						}
					}
					if (this._boatMpTriggers.Count == 0)
					{
						this._hasRaftEntity = false;
						base.state.RaftEntity = null;
					}
					else if (!this._hasRaftEntity)
					{
						BoltEntity componentInParent = this._boatMpTriggers[0].GetComponentInParent<BoltEntity>();
						base.state.RaftEntity = componentInParent;
						this._raftEntity = componentInParent;
						this._hasRaftEntity = this._raftEntity;
					}
				}
				if (this._hasRaftEntity)
				{
					this._raftTransform.position = this._raftEntity.transform.InverseTransformPoint(base.transform.position);
				}
			}
			else if (this._hasRaftEntity)
			{
				base.transform.position = this._raftEntity.transform.TransformPoint(this._raftTransform.position);
			}
		}
	}
}
