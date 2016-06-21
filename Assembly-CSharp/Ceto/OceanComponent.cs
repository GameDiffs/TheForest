using System;
using UnityEngine;

namespace Ceto
{
	[RequireComponent(typeof(Ocean))]
	public abstract class OceanComponent : MonoBehaviour
	{
		protected Ocean m_ocean;

		public bool WasError
		{
			get;
			protected set;
		}

		protected virtual void Awake()
		{
			try
			{
				this.m_ocean = base.GetComponent<Ocean>();
				this.m_ocean.Register(this);
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.WasError = true;
				base.enabled = false;
			}
		}

		protected virtual void OnEnable()
		{
			if (this.WasError || this.m_ocean == null || this.m_ocean.WasError)
			{
				base.enabled = false;
			}
		}

		protected virtual void OnDisable()
		{
		}

		protected virtual void OnDestroy()
		{
			try
			{
				this.m_ocean.Deregister(this);
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.WasError = true;
				base.enabled = false;
			}
		}

		public virtual void OceanOnPreRender(Camera cam, CameraData data)
		{
		}

		public virtual void OceanOnPreCull(Camera cam, CameraData data)
		{
		}

		public virtual void OceanOnPostRender(Camera cam, CameraData data)
		{
		}
	}
}
