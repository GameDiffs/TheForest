using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Sound Volume"), RequireComponent(typeof(UISlider))]
public class UISoundVolume : MonoBehaviour
{
	private void Awake()
	{
		UISlider component = base.GetComponent<UISlider>();
		component.value = NGUITools.soundVolume;
		EventDelegate.Add(component.onChange, new EventDelegate.Callback(this.OnChange));
	}

	private void OnChange()
	{
		NGUITools.soundVolume = UIProgressBar.current.value;
	}
}
