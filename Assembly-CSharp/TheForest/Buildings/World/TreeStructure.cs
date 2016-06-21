using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[AddComponentMenu("Buildings/World/Tree Structure (ex: Tree House)")]
	public class TreeStructure : EntityBehaviour<ITreeHouseState>
	{
		[SerializeThis]
		private int _treeId = -1;

		private TreeStructureLod _tsl;

		public int TreeId
		{
			get
			{
				return this._treeId;
			}
			set
			{
				this._treeId = value;
			}
		}

		private void Start()
		{
			if (!BoltNetwork.isClient)
			{
				if (!LevelSerializer.IsDeserializing)
				{
					base.StartCoroutine(this.OnDeserialized());
				}
			}
			else
			{
				base.enabled = false;
			}
		}

		private void OnDestroy()
		{
			if (this._tsl)
			{
				UnityEngine.Object.Destroy(this._tsl);
				this._tsl = null;
			}
		}

		[DebuggerHidden]
		public IEnumerator OnDeserialized()
		{
			TreeStructure.<OnDeserialized>c__Iterator14C <OnDeserialized>c__Iterator14C = new TreeStructure.<OnDeserialized>c__Iterator14C();
			<OnDeserialized>c__Iterator14C.<>f__this = this;
			return <OnDeserialized>c__Iterator14C;
		}

		[DebuggerHidden]
		private IEnumerator OnTreeCutDown(GameObject trunk)
		{
			TreeStructure.<OnTreeCutDown>c__Iterator14D <OnTreeCutDown>c__Iterator14D = new TreeStructure.<OnTreeCutDown>c__Iterator14D();
			<OnTreeCutDown>c__Iterator14D.trunk = trunk;
			<OnTreeCutDown>c__Iterator14D.<$>trunk = trunk;
			<OnTreeCutDown>c__Iterator14D.<>f__this = this;
			return <OnTreeCutDown>c__Iterator14D;
		}

		public override void Attached()
		{
			if (this.entity.isOwner)
			{
				if (this.entity.StateIs<ITreeHouseState>())
				{
					base.StartCoroutine(this.OnDeserialized());
				}
			}
			else if (this.entity.StateIs<ITreeHouseState>())
			{
				this.entity.GetState<ITreeHouseState>().AddCallback("TreeId", new PropertyCallbackSimple(this.OnReceivedTreeId));
			}
		}

		private void OnReceivedTreeId()
		{
			this._treeId = this.entity.GetState<ITreeHouseState>().TreeId;
			base.StartCoroutine(this.OnDeserialized());
			this.entity.GetState<ITreeHouseState>().RemoveCallback("TreeId", new PropertyCallbackSimple(this.OnReceivedTreeId));
		}
	}
}
