using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CoopWorldCrates : EntityBehaviour<IWorldCrates>
{
	[Header("World Creates (MAX 64 CURRENT)")]
	public BreakCrate[] Crates;

	public static CoopWorldCrates Instance;

	private float _lastSend;

	private void Awake()
	{
		CoopWorldCrates.Instance = this;
	}

	public override void Attached()
	{
		base.StartCoroutine(this.UpdateRoutine());
		base.state.AddCallback("Broken[]", new PropertyCallback(this.OnBroken));
	}

	private void OnBroken(IState _, string propertyPath, ArrayIndices arrayIndices)
	{
		for (int i = 0; i < this.Crates.Length; i++)
		{
			if (this.Crates[i] && base.state.Broken[i] == 1)
			{
				this.Crates[i].gameObject.SendMessage("ExplosionReal");
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator UpdateRoutine()
	{
		CoopWorldCrates.<UpdateRoutine>c__Iterator23 <UpdateRoutine>c__Iterator = new CoopWorldCrates.<UpdateRoutine>c__Iterator23();
		<UpdateRoutine>c__Iterator.<>f__this = this;
		return <UpdateRoutine>c__Iterator;
	}
}
