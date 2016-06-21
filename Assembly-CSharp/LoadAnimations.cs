using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

[AddComponentMenu("System/Load Animations")]
public class LoadAnimations : MonoBehaviour
{
	public new string name;

	private void Awake()
	{
		IEnumerable<AnimationClip> enumerable = Resources.LoadAll("Animations/" + this.name, typeof(AnimationClip)).Cast<AnimationClip>();
		foreach (AnimationClip current in enumerable)
		{
			base.GetComponent<Animation>().AddClip(current, (!current.name.Contains("@")) ? current.name : current.name.Substring(current.name.LastIndexOf("@") + 1));
		}
		foreach (AnimationState current2 in base.GetComponent<Animation>().Cast<AnimationState>())
		{
			current2.enabled = true;
		}
	}
}
