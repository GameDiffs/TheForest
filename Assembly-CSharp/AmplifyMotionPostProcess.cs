using System;
using UnityEngine;

[AddComponentMenu(""), RequireComponent(typeof(Camera))]
public sealed class AmplifyMotionPostProcess : MonoBehaviour
{
	private AmplifyMotionEffectBase m_instance;

	public AmplifyMotionEffectBase Instance
	{
		get
		{
			return this.m_instance;
		}
		set
		{
			this.m_instance = value;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.m_instance != null)
		{
			this.m_instance.PostProcess(source, destination);
		}
	}
}
