using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AmplifyMotion
{
	internal class SolidState : MotionState
	{
		public MeshRenderer m_meshRenderer;

		public Matrix4x4 m_prevLocalToWorld;

		public Matrix4x4 m_currLocalToWorld;

		public Vector3 m_lastPosition;

		public Quaternion m_lastRotation;

		public Vector3 m_lastScale;

		private Mesh m_mesh;

		private MotionState.MaterialDesc[] m_sharedMaterials;

		public bool m_moved;

		private bool m_wasVisible;

		private static HashSet<AmplifyMotionObjectBase> m_uniqueWarnings = new HashSet<AmplifyMotionObjectBase>();

		public SolidState(AmplifyMotionCamera owner, AmplifyMotionObjectBase obj) : base(owner, obj)
		{
			this.m_meshRenderer = this.m_obj.GetComponent<MeshRenderer>();
		}

		internal override void Initialize()
		{
			MeshFilter component = this.m_obj.GetComponent<MeshFilter>();
			if (component == null || component.mesh == null)
			{
				if (!SolidState.m_uniqueWarnings.Contains(this.m_obj))
				{
					Debug.LogWarning("[AmplifyMotion] Invalid MeshFilter/Mesh in object " + this.m_obj.name + ". Skipping.");
					SolidState.m_uniqueWarnings.Add(this.m_obj);
				}
				this.m_error = true;
				return;
			}
			base.Initialize();
			this.m_mesh = component.mesh;
			this.m_sharedMaterials = base.ProcessSharedMaterials(this.m_meshRenderer.sharedMaterials);
			this.m_wasVisible = false;
		}

		internal override void UpdateTransform(CommandBuffer updateCB, bool starting)
		{
			if (!this.m_initialized)
			{
				this.Initialize();
				return;
			}
			if (!starting && this.m_wasVisible)
			{
				this.m_prevLocalToWorld = this.m_currLocalToWorld;
			}
			this.m_moved = true;
			if (!this.m_owner.Overlay)
			{
				Vector3 position = this.m_transform.position;
				Quaternion rotation = this.m_transform.rotation;
				Vector3 lossyScale = this.m_transform.lossyScale;
				this.m_moved = (starting || MotionState.VectorChanged(position, this.m_lastPosition) || MotionState.RotationChanged(rotation, this.m_lastRotation) || MotionState.VectorChanged(lossyScale, this.m_lastScale));
				if (this.m_moved)
				{
					this.m_lastPosition = position;
					this.m_lastRotation = rotation;
					this.m_lastScale = lossyScale;
				}
			}
			this.m_currLocalToWorld = this.m_transform.localToWorldMatrix;
			if (starting || !this.m_wasVisible)
			{
				this.m_prevLocalToWorld = this.m_currLocalToWorld;
			}
			this.m_wasVisible = this.m_meshRenderer.isVisible;
		}

		internal override void RenderVectors(Camera camera, CommandBuffer renderCB, float scale, Quality quality)
		{
			if (this.m_initialized && !this.m_error && this.m_meshRenderer.isVisible)
			{
				bool flag = (this.m_owner.Instance.CullingMask & 1 << this.m_obj.gameObject.layer) != 0;
				if (!flag || (flag && this.m_moved))
				{
					int num = (!flag) ? 255 : this.m_owner.Instance.GenerateObjectId(this.m_obj.gameObject);
					Matrix4x4 value;
					if (this.m_obj.FixedStep)
					{
						value = this.m_owner.PrevViewProjMatrixRT * this.m_currLocalToWorld;
					}
					else
					{
						value = this.m_owner.PrevViewProjMatrixRT * this.m_prevLocalToWorld;
					}
					renderCB.SetGlobalMatrix("_AM_MATRIX_PREV_MVP", value);
					renderCB.SetGlobalFloat("_AM_OBJECT_ID", (float)num * 0.003921569f);
					renderCB.SetGlobalFloat("_AM_MOTION_SCALE", (!flag) ? 0f : scale);
					int num2 = (quality != Quality.Mobile) ? 2 : 0;
					for (int i = 0; i < this.m_sharedMaterials.Length; i++)
					{
						MotionState.MaterialDesc materialDesc = this.m_sharedMaterials[i];
						int shaderPass = num2 + ((!materialDesc.coverage) ? 0 : 1);
						if (materialDesc.coverage)
						{
							Texture mainTexture = materialDesc.material.mainTexture;
							if (mainTexture != null)
							{
								materialDesc.propertyBlock.SetTexture("_MainTex", mainTexture);
							}
							if (materialDesc.cutoff)
							{
								materialDesc.propertyBlock.SetFloat("_Cutoff", materialDesc.material.GetFloat("_Cutoff"));
							}
						}
						renderCB.DrawMesh(this.m_mesh, this.m_transform.localToWorldMatrix, this.m_owner.Instance.SolidVectorsMaterial, i, shaderPass, materialDesc.propertyBlock);
					}
				}
			}
		}
	}
}
