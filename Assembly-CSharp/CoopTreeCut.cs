using Bolt;
using System;
using UniLinq;
using UnityEngine;

public class CoopTreeCut : CoopBase<ITreeCutState>
{
	private TreeCutChunk[] chunks;

	[HideInInspector]
	public CoopTreeId CoopTree;

	private PropertyCallbackSimple ChunkUpdater(int c, Func<int> value)
	{
		return delegate
		{
			switch (value())
			{
			case 1:
				this.chunks[c].Fake1.SetActive(false);
				this.chunks[c].Fake2.SetActive(true);
				break;
			case 2:
				this.chunks[c].Fake1.SetActive(false);
				this.chunks[c].Fake2.SetActive(false);
				this.chunks[c].Fake3.SetActive(true);
				break;
			case 3:
				this.chunks[c].Fake1.SetActive(false);
				this.chunks[c].Fake2.SetActive(false);
				this.chunks[c].Fake3.SetActive(false);
				this.chunks[c].Fake4.SetActive(true);
				break;
			case 4:
				this.chunks[c].Fake1.SetActive(false);
				this.chunks[c].Fake2.SetActive(false);
				this.chunks[c].Fake3.SetActive(false);
				this.chunks[c].Fake4.SetActive(false);
				this.chunks[c].transform.parent.gameObject.SetActive(false);
				break;
			}
		};
	}

	public override void Attached()
	{
		this.chunks = (from x in base.GetComponentsInChildren<TreeCutChunk>()
		orderby int.Parse(x.transform.parent.gameObject.name)
		select x).ToArray<TreeCutChunk>();
		base.state.AddCallback("TreeId", delegate
		{
			CoopTreeId coopTreeId = CoopPlayerCallbacks.AllTrees.FirstOrDefault((CoopTreeId x) => x.Id == base.state.TreeId);
			if (coopTreeId)
			{
				LOD_Trees component = coopTreeId.GetComponent<LOD_Trees>();
				if (component)
				{
					component.enabled = false;
					component.DontSpawn = true;
					if (component.CurrentView)
					{
						component.Pool.Despawn(component.CurrentView.transform);
						component.CurrentView = null;
						component.CurrentLodTransform = null;
					}
				}
			}
		});
		base.state.AddCallback("Chunk1", this.ChunkUpdater(0, () => base.state.Chunk1));
		base.state.AddCallback("Chunk2", this.ChunkUpdater(1, () => base.state.Chunk2));
		base.state.AddCallback("Chunk3", this.ChunkUpdater(2, () => base.state.Chunk3));
		base.state.AddCallback("Chunk4", this.ChunkUpdater(3, () => base.state.Chunk4));
		base.state.AddCallback("Damage", delegate
		{
			if (this.entity.isOwner && base.state.Damage == 16)
			{
				this.entity.DestroyDelayed(10f);
				BoltEntity boltEntity = BoltNetwork.Instantiate(this.CoopTree.NetworkFallPrefab, this.entity.transform.position, this.entity.transform.rotation);
				boltEntity.GetState<ITreeFallState>().CutTree = this.entity;
				boltEntity.GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.value * 0.01f, 0f, UnityEngine.Random.value * 0.01f), ForceMode.VelocityChange);
			}
		});
	}
}
