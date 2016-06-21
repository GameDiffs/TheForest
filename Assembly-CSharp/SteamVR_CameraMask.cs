using System;
using UnityEngine;
using UnityEngine.Rendering;
using Valve.VR;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SteamVR_CameraMask : MonoBehaviour
{
	private static Material material;

	private static Mesh[] hiddenAreaMeshes = new Mesh[2];

	private MeshFilter meshFilter;

	private void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		if (SteamVR_CameraMask.material == null)
		{
			SteamVR_CameraMask.material = new Material(Shader.Find("Custom/SteamVR_HiddenArea"));
		}
		MeshRenderer component = base.GetComponent<MeshRenderer>();
		component.material = SteamVR_CameraMask.material;
		component.shadowCastingMode = ShadowCastingMode.Off;
		component.receiveShadows = false;
		component.useLightProbes = false;
		component.reflectionProbeUsage = ReflectionProbeUsage.Off;
	}

	public void Set(SteamVR vr, EVREye eye)
	{
		if (SteamVR_CameraMask.hiddenAreaMeshes[(int)eye] == null)
		{
			SteamVR_CameraMask.hiddenAreaMeshes[(int)eye] = SteamVR_Utils.CreateHiddenAreaMesh(vr.hmd.GetHiddenAreaMesh(eye), vr.textureBounds[(int)eye]);
		}
		this.meshFilter.mesh = SteamVR_CameraMask.hiddenAreaMeshes[(int)eye];
	}

	public void Clear()
	{
		this.meshFilter.mesh = null;
	}
}
