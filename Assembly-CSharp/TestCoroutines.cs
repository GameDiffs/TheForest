using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("Storage/Tests/Coroutines")]
public class TestCoroutines : MonoBehaviour
{
	private void Start()
	{
		if (!LevelSerializer.IsDeserializing)
		{
			base.gameObject.StartExtendedCoroutine(this.MyCoroutine());
		}
		base.StartCoroutine("Hello");
	}

	[DebuggerHidden]
	private IEnumerator Hello()
	{
		TestCoroutines.<Hello>c__Iterator210 <Hello>c__Iterator = new TestCoroutines.<Hello>c__Iterator210();
		<Hello>c__Iterator.<>f__this = this;
		return <Hello>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator MyCoroutine()
	{
		TestCoroutines.<MyCoroutine>c__Iterator211 <MyCoroutine>c__Iterator = new TestCoroutines.<MyCoroutine>c__Iterator211();
		<MyCoroutine>c__Iterator.<>f__this = this;
		return <MyCoroutine>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator WaitSeconds(float time)
	{
		TestCoroutines.<WaitSeconds>c__Iterator212 <WaitSeconds>c__Iterator = new TestCoroutines.<WaitSeconds>c__Iterator212();
		<WaitSeconds>c__Iterator.time = time;
		<WaitSeconds>c__Iterator.<$>time = time;
		return <WaitSeconds>c__Iterator;
	}
}
