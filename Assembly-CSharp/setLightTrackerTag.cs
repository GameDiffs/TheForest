using System;
using UnityEngine;

public class setLightTrackerTag : MonoBehaviour
{
	private void Start()
	{
		base.Invoke("setTrackerTag", 0.2f);
	}

	private void setTrackerTag()
	{
		Transform[] componentsInChildren = base.transform.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i];
			transform.gameObject.tag = "Player";
		}
	}
}
