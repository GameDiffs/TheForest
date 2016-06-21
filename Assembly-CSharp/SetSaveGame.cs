using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class SetSaveGame : MonoBehaviour
{
	public int Current = 1;

	public int MinVersion = 31;

	[DebuggerHidden]
	private IEnumerator Start()
	{
		SetSaveGame.<Start>c__Iterator1A9 <Start>c__Iterator1A = new SetSaveGame.<Start>c__Iterator1A9();
		<Start>c__Iterator1A.<>f__this = this;
		return <Start>c__Iterator1A;
	}
}
