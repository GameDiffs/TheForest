using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class batFlyOutCave : MonoBehaviour
{
	public bool bat1;

	public bool bat2;

	public bool bat3;

	public bool bat4;

	public bool bat5;

	public bool bat6;

	public bool bat7;

	public bool bat8;

	public bool bat9;

	public bool bat10;

	public bool bat11;

	public bool bat12;

	public bool bat13;

	public bool bat14;

	public bool bat15;

	public bool bat16;

	private Animator anim;

	private void Awake()
	{
		this.anim = base.GetComponent<Animator>();
	}

	[DebuggerHidden]
	private IEnumerator doBats()
	{
		batFlyOutCave.<doBats>c__Iterator214 <doBats>c__Iterator = new batFlyOutCave.<doBats>c__Iterator214();
		<doBats>c__Iterator.<>f__this = this;
		return <doBats>c__Iterator;
	}
}
