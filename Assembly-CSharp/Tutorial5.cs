using System;
using UnityEngine;

public class Tutorial5 : MonoBehaviour
{
	public void SetDurationToCurrentProgress()
	{
		UITweener[] componentsInChildren = base.GetComponentsInChildren<UITweener>();
		UITweener[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			UITweener uITweener = array[i];
			uITweener.duration = Mathf.Lerp(2f, 0.5f, UIProgressBar.current.value);
		}
	}
}
