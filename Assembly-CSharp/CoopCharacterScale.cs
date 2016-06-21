using Bolt;
using System;
using UnityEngine;

public class CoopCharacterScale : EntityBehaviour<IWorldCharacter>
{
	public Transform ApplyTo;

	private void Awake()
	{
		if (!BoltNetwork.isRunning)
		{
			base.enabled = false;
		}
	}

	public override void Attached()
	{
		if (!this.entity.IsOwner())
		{
			base.state.AddCallback("CharacterScale", new PropertyCallbackSimple(this.OnCharacterScaleChanged));
		}
	}

	private void Update()
	{
		if (this.entity.IsOwner())
		{
			base.state.CharacterScale = this.ApplyTo.localScale;
		}
	}

	private void OnCharacterScaleChanged()
	{
		this.ApplyTo.localScale = base.state.CharacterScale;
	}
}
