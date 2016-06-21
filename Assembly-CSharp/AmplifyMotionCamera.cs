using AmplifyMotion;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[AddComponentMenu(""), RequireComponent(typeof(Camera))]
public class AmplifyMotionCamera : MonoBehaviour
{
	internal AmplifyMotionEffectBase Instance;

	internal Matrix4x4 PrevViewProjMatrix;

	internal Matrix4x4 ViewProjMatrix;

	internal Matrix4x4 InvViewProjMatrix;

	internal Matrix4x4 PrevViewProjMatrixRT;

	internal Matrix4x4 ViewProjMatrixRT;

	internal Transform Transform;

	private bool m_linked;

	private bool m_initialized;

	private bool m_starting = true;

	private bool m_autoStep = true;

	private bool m_step;

	private bool m_overlay;

	private Camera m_camera;

	private int m_prevFrameCount;

	private HashSet<AmplifyMotionObjectBase> m_affectedObjectsTable = new HashSet<AmplifyMotionObjectBase>();

	private AmplifyMotionObjectBase[] m_affectedObjects;

	private bool m_affectedObjectsChanged = true;

	public bool Initialized
	{
		get
		{
			return this.m_initialized;
		}
	}

	public bool AutoStep
	{
		get
		{
			return this.m_autoStep;
		}
	}

	public bool Overlay
	{
		get
		{
			return this.m_overlay;
		}
	}

	public Camera Camera
	{
		get
		{
			return this.m_camera;
		}
	}

	public void RegisterObject(AmplifyMotionObjectBase obj)
	{
		this.m_affectedObjectsTable.Add(obj);
		this.m_affectedObjectsChanged = true;
	}

	public void UnregisterObject(AmplifyMotionObjectBase obj)
	{
		this.m_affectedObjectsTable.Remove(obj);
		this.m_affectedObjectsChanged = true;
	}

	private void UpdateAffectedObjects()
	{
		if (this.m_affectedObjects == null || this.m_affectedObjectsTable.Count != this.m_affectedObjects.Length)
		{
			this.m_affectedObjects = new AmplifyMotionObjectBase[this.m_affectedObjectsTable.Count];
		}
		this.m_affectedObjectsTable.CopyTo(this.m_affectedObjects);
		this.m_affectedObjectsChanged = false;
	}

	public void LinkTo(AmplifyMotionEffectBase instance, bool overlay)
	{
		this.Instance = instance;
		this.m_camera = base.GetComponent<Camera>();
		this.m_camera.depthTextureMode |= DepthTextureMode.Depth;
		this.m_overlay = overlay;
		this.m_linked = true;
	}

	public void Initialize()
	{
		this.m_step = false;
		this.UpdateMatrices();
		this.m_initialized = true;
	}

	private void Awake()
	{
		this.Transform = base.transform;
	}

	private void OnEnable()
	{
		AmplifyMotionEffectBase.RegisterCamera(this);
	}

	private void OnDisable()
	{
		this.m_initialized = false;
		AmplifyMotionEffectBase.UnregisterCamera(this);
	}

	private void OnDestroy()
	{
		if (this.Instance != null)
		{
			this.Instance.RemoveCamera(this.m_camera);
		}
	}

	public void StopAutoStep()
	{
		if (this.m_autoStep)
		{
			this.m_autoStep = false;
			this.m_step = true;
		}
	}

	public void StartAutoStep()
	{
		this.m_autoStep = true;
	}

	public void Step()
	{
		this.m_step = true;
	}

	private void Update()
	{
		if (!this.m_linked || !this.Instance.isActiveAndEnabled)
		{
			return;
		}
		if (!this.m_initialized)
		{
			this.Initialize();
		}
		if ((this.m_camera.depthTextureMode & DepthTextureMode.Depth) == DepthTextureMode.None)
		{
			this.m_camera.depthTextureMode |= DepthTextureMode.Depth;
		}
	}

	private void UpdateMatrices()
	{
		if (!this.m_starting)
		{
			this.PrevViewProjMatrix = this.ViewProjMatrix;
			this.PrevViewProjMatrixRT = this.ViewProjMatrixRT;
		}
		Matrix4x4 worldToCameraMatrix = this.m_camera.worldToCameraMatrix;
		Matrix4x4 gPUProjectionMatrix = GL.GetGPUProjectionMatrix(this.m_camera.projectionMatrix, false);
		this.ViewProjMatrix = gPUProjectionMatrix * worldToCameraMatrix;
		this.InvViewProjMatrix = Matrix4x4.Inverse(this.ViewProjMatrix);
		Matrix4x4 gPUProjectionMatrix2 = GL.GetGPUProjectionMatrix(this.m_camera.projectionMatrix, true);
		this.ViewProjMatrixRT = gPUProjectionMatrix2 * worldToCameraMatrix;
		if (this.m_starting)
		{
			this.PrevViewProjMatrix = this.ViewProjMatrix;
			this.PrevViewProjMatrixRT = this.ViewProjMatrixRT;
		}
	}

	public void FixedUpdateTransform(CommandBuffer updateCB)
	{
		if (!this.m_initialized)
		{
			this.Initialize();
		}
		if (this.m_affectedObjectsChanged)
		{
			this.UpdateAffectedObjects();
		}
		for (int i = 0; i < this.m_affectedObjects.Length; i++)
		{
			if (this.m_affectedObjects[i].FixedStep)
			{
				this.m_affectedObjects[i].OnUpdateTransform(this.m_camera, updateCB, this.m_starting);
			}
		}
	}

	public void UpdateTransform(CommandBuffer updateCB)
	{
		if (!this.m_initialized)
		{
			this.Initialize();
		}
		if (Time.frameCount > this.m_prevFrameCount && (this.m_autoStep || this.m_step))
		{
			this.UpdateMatrices();
			if (this.m_affectedObjectsChanged)
			{
				this.UpdateAffectedObjects();
			}
			for (int i = 0; i < this.m_affectedObjects.Length; i++)
			{
				if (!this.m_affectedObjects[i].FixedStep)
				{
					this.m_affectedObjects[i].OnUpdateTransform(this.m_camera, updateCB, this.m_starting);
				}
			}
			this.m_starting = false;
			this.m_step = false;
			this.m_prevFrameCount = Time.frameCount;
		}
	}

	public void RenderVectors(CommandBuffer renderCB, float scale, float fixedScale, Quality quality)
	{
		if (!this.m_initialized)
		{
			this.Initialize();
		}
		float nearClipPlane = this.m_camera.nearClipPlane;
		float farClipPlane = this.m_camera.farClipPlane;
		Vector4 vec;
		if (AmplifyMotionEffectBase.IsD3D)
		{
			vec.x = 1f - farClipPlane / nearClipPlane;
			vec.y = farClipPlane / nearClipPlane;
		}
		else
		{
			vec.x = (1f - farClipPlane / nearClipPlane) / 2f;
			vec.y = (1f + farClipPlane / nearClipPlane) / 2f;
		}
		vec.z = vec.x / farClipPlane;
		vec.w = vec.y / farClipPlane;
		Shader.SetGlobalVector("_AM_ZBUFFER_PARAMS", vec);
		if (this.m_affectedObjectsChanged)
		{
			this.UpdateAffectedObjects();
		}
		for (int i = 0; i < this.m_affectedObjects.Length; i++)
		{
			if ((this.m_camera.cullingMask & 1 << this.m_affectedObjects[i].gameObject.layer) != 0)
			{
				this.m_affectedObjects[i].OnRenderVectors(this.m_camera, renderCB, (!this.m_affectedObjects[i].FixedStep) ? scale : fixedScale, quality);
			}
		}
	}

	private void OnGUI()
	{
		if (!Application.isEditor)
		{
			return;
		}
		if (!this.m_linked || !this.Instance.isActiveAndEnabled)
		{
			return;
		}
		if (!this.m_initialized)
		{
			this.Initialize();
		}
		if (this.m_affectedObjectsChanged)
		{
			this.UpdateAffectedObjects();
		}
		for (int i = 0; i < this.m_affectedObjects.Length; i++)
		{
			this.m_affectedObjects[i].OnRenderDebugHUD(this.m_camera);
		}
	}
}
