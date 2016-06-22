using System;
using UnityEngine;

[Serializable]
public class gameSettings : MonoBehaviour
{
	[NonSerialized]
	public static ThreadPriority backgroundLoadingPriority;

	public override void Start()
	{
		Application.backgroundLoadingPriority = ThreadPriority.Low;
	}

	public override void Main()
	{
	}
}
