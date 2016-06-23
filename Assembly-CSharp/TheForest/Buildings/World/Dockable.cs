using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Buildings.Creation;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic]
	public class Dockable : EntityBehaviour<IRaftState>
	{
		public CoopRaftPusher2 _pusher;

		public GameObject _dockIcon;

		public Rigidbody _dockableRb;

		public Transform _ropeRoot;

		public Transform _ropeTr;

		public Bounds _bounds;

		private OnDestroyProxy _destroyProxy;

		private GameObject _dockStilt;

		private bool _atDock;

		[SerializeThis]
		private bool _docked;

		private bool _forceDock;

		private List<Collider> _dockTriggers = new List<Collider>();

		private void Awake()
		{
			base.enabled = false;
		}

		private void Update()
		{
			if (TheForest.Utils.Input.GetButtonAfterDelay("Take", 0.5f))
			{
				if (!this._docked)
				{
					if (BoltNetwork.isRunning)
					{
						this.MpSendDock(this._dockStilt.transform.position);
					}
					else
					{
						this.Dock();
					}
				}
				else if (BoltNetwork.isRunning)
				{
					this.MpSendDock(Vector3.zero);
				}
				else
				{
					this.UnDock();
				}
			}
			if (!this._atDock)
			{
				base.enabled = false;
				this.UnDock();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Grabber"))
			{
				if (this._atDock && this.ValidateTriggers())
				{
					base.enabled = true;
					this._dockIcon.SetActive(true);
					base.SendMessage("GrabEnter");
				}
			}
			else if (!this._docked && this.IsDock(other))
			{
				this.AddDockCollider(other);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Grabber"))
			{
				base.enabled = false;
				this._dockIcon.SetActive(false);
				base.SendMessage("GrabExit");
			}
			else if (this.IsDock(other))
			{
				this.RemoveDockCollider(other);
			}
		}

		private void OnDestroy()
		{
			this.ClearOutDestroyProxy();
		}

		[DebuggerHidden]
		private IEnumerator OnDeserialized()
		{
			Dockable.<OnDeserialized>c__Iterator14D <OnDeserialized>c__Iterator14D = new Dockable.<OnDeserialized>c__Iterator14D();
			<OnDeserialized>c__Iterator14D.<>f__this = this;
			return <OnDeserialized>c__Iterator14D;
		}

		private void OnDockStiltDestroyed()
		{
			if (BoltNetwork.isRunning)
			{
				this.MpSendDock(Vector3.zero);
			}
			else
			{
				this.UnDock();
			}
		}

		private bool IsDock(Collider c)
		{
			PrefabIdentifier componentInParent = c.GetComponentInParent<PrefabIdentifier>();
			return componentInParent && componentInParent.GetComponent<DockArchitect>();
		}

		private void AddDockCollider(Collider c)
		{
			if (!this._dockTriggers.Contains(c))
			{
				this._dockTriggers.Add(c);
			}
			this.SetStilt(c.gameObject);
		}

		private void RemoveDockCollider(Collider c)
		{
			if (this._dockTriggers.Contains(c))
			{
				this._dockTriggers.Remove(c);
			}
			if (this._dockStilt == c.gameObject)
			{
				if (this._dockTriggers.Count > 0)
				{
					this.SetStilt(this._dockTriggers[this._dockTriggers.Count - 1].gameObject);
				}
				else
				{
					this.UnsetStilt();
				}
			}
		}

		private void SetStilt(GameObject stilt)
		{
			this._atDock = true;
			this._dockStilt = stilt;
			Vector3 position = this._dockStilt.transform.position + (base.transform.position - this._dockStilt.transform.position).normalized * 1f;
			position.y = base.transform.position.y + 0.5f;
			this._dockIcon.transform.position = position;
			if (this._forceDock && (!BoltNetwork.isRunning || this.entity.isAttached))
			{
				this._forceDock = false;
				this.Dock();
			}
		}

		private void UnsetStilt()
		{
			base.enabled = false;
			this._atDock = false;
			this._dockStilt = null;
			if (this._docked)
			{
				this.UnDock();
			}
		}

		private void Dock()
		{
			this._pusher.enabled = false;
			this._docked = true;
			this._dockableRb.constraints = RigidbodyConstraints.FreezeAll;
			this.ClearOutDestroyProxy();
			this._destroyProxy = this._dockStilt.AddComponent<OnDestroyProxy>();
			this._destroyProxy._todo = this;
			this._destroyProxy._message = "OnDockStiltDestroyed";
			this._ropeRoot.position = this._dockStilt.transform.position;
			Vector3 worldPosition = this._dockStilt.transform.position + (base.transform.position - this._dockStilt.transform.position).normalized * 1.75f;
			worldPosition.y = this._dockStilt.transform.position.y;
			this._ropeRoot.LookAt(worldPosition);
			worldPosition.y = base.transform.position.y + 0.5f;
			this._ropeTr.LookAt(worldPosition);
			this._ropeRoot.gameObject.SetActive(true);
		}

		private void UnDock()
		{
			this._docked = false;
			this._dockableRb.constraints = RigidbodyConstraints.None;
			this._pusher.enabled = true;
			this._ropeRoot.gameObject.SetActive(false);
			this.ClearOutDestroyProxy();
		}

		private void ClearOutDestroyProxy()
		{
			if (this._destroyProxy)
			{
				UnityEngine.Object.Destroy(this._destroyProxy);
				this._destroyProxy = null;
			}
		}

		private bool ValidateTriggers()
		{
			if (this._dockTriggers.Count > 0)
			{
				Bounds bounds = new Bounds(base.transform.TransformPoint(this._bounds.center), this._bounds.size);
				for (int i = this._dockTriggers.Count - 1; i >= 0; i--)
				{
					if (!this._dockTriggers[i])
					{
						this._dockTriggers.RemoveAt(i);
					}
					else if (!this._dockTriggers[i].bounds.Intersects(bounds))
					{
						this.OnTriggerExit(this._dockTriggers[i]);
					}
				}
			}
			return this._dockTriggers.Count > 0;
		}

		public override void Attached()
		{
			base.state.AddCallback("DockPosition", new PropertyCallbackSimple(this.DockPositionUpdated));
			if (this.entity.isOwner)
			{
				if (this._forceDock && this._dockStilt)
				{
					this._forceDock = false;
					this.MpSendDock(this._dockStilt.transform.position);
				}
			}
			else
			{
				base.Invoke("DockPositionUpdated", 1f);
			}
		}

		public void MpSendDock(Vector3 position)
		{
			if (!this.entity.isOwner)
			{
				ToggleDockingState toggleDockingState = ToggleDockingState.Create(this.entity.source);
				toggleDockingState.DockPosition = position;
				toggleDockingState.Entity = this.entity;
				toggleDockingState.Send();
			}
			else
			{
				base.state.DockPosition = position;
			}
		}

		private void DockPositionUpdated()
		{
			if (base.state.DockPosition == Vector3.zero)
			{
				this.UnDock();
			}
			else
			{
				Collider[] array = Physics.OverlapSphere(base.state.DockPosition, 0.5f, 2097152);
				if (array.Length > 0)
				{
					Collider[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						Collider collider = array2[i];
						if (this.IsDock(collider) && !this._dockTriggers.Contains(collider))
						{
							this._dockTriggers.Add(collider);
						}
					}
					if (this._dockTriggers.Count > 0)
					{
						this._forceDock = true;
						this.SetStilt(this._dockTriggers[0].gameObject);
					}
				}
			}
		}
	}
}
