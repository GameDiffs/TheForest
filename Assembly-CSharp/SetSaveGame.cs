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
		SetSaveGame.<Start>c__Iterator1B1 <Start>c__Iterator1B = new SetSaveGame.<Start>c__Iterator1B1();
		<Start>c__Iterator1B.<>f__this = this;
		return <Start>c__Iterator1B;
	}
}
