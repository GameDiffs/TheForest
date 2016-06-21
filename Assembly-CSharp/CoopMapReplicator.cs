using Bolt;
using System;
using UnityEngine;

public class CoopMapReplicator : EntityBehaviour<IPlayerState>
{
	public GameObject[] MapPieces;

	public override void Attached()
	{
		if (!this.entity.isOwner)
		{
			base.state.AddCallback("MapPieces", new PropertyCallbackSimple(this.MapPiecesChanged));
		}
	}

	private void Update()
	{
		if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner)
		{
			for (int i = 0; i < this.MapPieces.Length; i++)
			{
				int num = (!this.MapPieces[i].activeInHierarchy) ? 0 : 1;
				if (num != base.state.MapPieces[i])
				{
					base.state.MapPieces[i] = num;
				}
			}
		}
	}

	private void MapPiecesChanged()
	{
		for (int i = 0; i < this.MapPieces.Length; i++)
		{
			this.MapPieces[i].SetActive(base.state.MapPieces[i] == 1);
		}
	}
}
