using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Amplify Motion"), RequireComponent(typeof(Camera))]
public class AmplifyMotionEffect : AmplifyMotionEffectBase
{
	public new static AmplifyMotionEffect FirstInstance
	{
		get
		{
			return (AmplifyMotionEffect)AmplifyMotionEffectBase.FirstInstance;
		}
	}

	public new static AmplifyMotionEffect Instance
	{
		get
		{
			return (AmplifyMotionEffect)AmplifyMotionEffectBase.Instance;
		}
	}
}
