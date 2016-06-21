using AmplifyMotion;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[AddComponentMenu("")]
public class AmplifyMotionObjectBase : MonoBehaviour
{
	public enum MinMaxCurveState
	{
		Scalar,
		Curve,
		TwoCurves,
		TwoScalars
	}

	internal static bool ApplyToChildren = true;

	[SerializeField]
	private bool m_applyToChildren = AmplifyMotionObjectBase.ApplyToChildren;

	public bool skipUpdate;

	private bool wait1Frame;

	private amplifyDisableHook adh;

	private ObjectType m_type;

	private Dictionary<Camera, MotionState> m_states = new Dictionary<Camera, MotionState>();

	private bool m_fixedStep;

	private int m_objectId;

	internal bool FixedStep
	{
		get
		{
			return this.m_fixedStep;
		}
	}

	internal int ObjectId
	{
		get
		{
			return this.m_objectId;
		}
	}

	public ObjectType Type
	{
		get
		{
			return this.m_type;
		}
	}

	internal void RegisterCamera(AmplifyMotionCamera camera)
	{
		Camera component = camera.GetComponent<Camera>();
		if ((component.cullingMask & 1 << base.gameObject.layer) != 0 && !this.m_states.ContainsKey(component))
		{
			MotionState value;
			switch (this.m_type)
			{
			case ObjectType.Solid:
				value = new SolidState(camera, this);
				break;
			case ObjectType.Skinned:
				value = new SkinnedState(camera, this);
				break;
			case ObjectType.Cloth:
				value = new ClothState(camera, this);
				break;
			default:
				throw new Exception("[AmplifyMotion] Invalid object type.");
			}
			camera.RegisterObject(this);
			this.m_states.Add(component, value);
		}
	}

	internal void UnregisterCamera(AmplifyMotionCamera camera)
	{
		Camera component = camera.GetComponent<Camera>();
		MotionState motionState;
		if (this.m_states.TryGetValue(component, out motionState))
		{
			camera.UnregisterObject(this);
			if (this.m_states.TryGetValue(component, out motionState))
			{
				motionState.Shutdown();
			}
			this.m_states.Remove(component);
		}
	}

	private bool InitializeType()
	{
		Renderer component = base.GetComponent<Renderer>();
		if (AmplifyMotionEffectBase.CanRegister(base.gameObject, false) && component != null)
		{
			if (component.GetType() == typeof(MeshRenderer))
			{
				this.m_type = ObjectType.Solid;
			}
			else if (component.GetType() == typeof(SkinnedMeshRenderer))
			{
				if (base.GetComponent<Cloth>() != null)
				{
					this.m_type = ObjectType.Cloth;
				}
				else
				{
					this.m_type = ObjectType.Skinned;
				}
			}
			AmplifyMotionEffectBase.RegisterObject(this);
		}
		return component != null;
	}

	private void OnEnable()
	{
		bool flag = this.InitializeType();
		if (flag)
		{
			if (this.m_type == ObjectType.Cloth)
			{
				this.m_fixedStep = false;
			}
			else if (this.m_type == ObjectType.Solid)
			{
				Rigidbody component = base.GetComponent<Rigidbody>();
				if (component != null && component.interpolation == RigidbodyInterpolation.None && !component.isKinematic)
				{
					this.m_fixedStep = true;
				}
			}
		}
		if (this.m_applyToChildren)
		{
			foreach (Transform transform in base.gameObject.transform)
			{
				AmplifyMotionEffectBase.RegisterRecursivelyS(transform.gameObject);
			}
		}
		if (!flag)
		{
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		AmplifyMotionEffectBase.UnregisterObject(this);
	}

	private void TryInitializeStates()
	{
		Dictionary<Camera, MotionState>.Enumerator enumerator = this.m_states.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<Camera, MotionState> current = enumerator.Current;
			MotionState value = current.Value;
			if (value.Owner.Initialized && !value.Error && !value.Initialized)
			{
				value.Initialize();
			}
		}
	}

	private void Start()
	{
		this.adh = base.transform.GetComponentInChildren<amplifyDisableHook>();
		if (AmplifyMotionEffectBase.Instance != null)
		{
			this.TryInitializeStates();
		}
	}

	private void Update()
	{
		if (AmplifyMotionEffectBase.Instance != null)
		{
			this.TryInitializeStates();
		}
	}

	internal void OnUpdateTransform(Camera camera, CommandBuffer updateCB, bool starting)
	{
		if (this.adh && this.adh.skipUpdate)
		{
			return;
		}
		MotionState motionState;
		if (this.m_states.TryGetValue(camera, out motionState) && !motionState.Error)
		{
			if (this.adh)
			{
				if (!this.adh.skipUpdate)
				{
					motionState.UpdateTransform(updateCB, starting);
				}
			}
			else
			{
				motionState.UpdateTransform(updateCB, starting);
			}
		}
	}

	internal void OnRenderVectors(Camera camera, CommandBuffer renderCB, float scale, Quality quality)
	{
		if (this.adh && this.adh.skipUpdate)
		{
			this.wait1Frame = true;
			return;
		}
		MotionState motionState;
		if (this.m_states.TryGetValue(camera, out motionState) && !motionState.Error)
		{
			if (this.adh)
			{
				if (!this.adh.skipUpdate)
				{
					if (this.wait1Frame)
					{
						this.wait1Frame = false;
						return;
					}
					motionState.RenderVectors(camera, renderCB, scale, quality);
				}
			}
			else
			{
				motionState.RenderVectors(camera, renderCB, scale, quality);
			}
		}
	}

	internal void OnRenderDebugHUD(Camera camera)
	{
		MotionState motionState;
		if (this.m_states.TryGetValue(camera, out motionState) && !motionState.Error)
		{
			motionState.RenderDebugHUD();
		}
	}
}
