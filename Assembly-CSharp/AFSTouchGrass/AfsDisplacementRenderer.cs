using System;
using UnityEngine;

namespace AFSTouchGrass
{
	[RequireComponent(typeof(TrailRenderer))]
	public class AfsDisplacementRenderer : MonoBehaviour
	{
		public enum UpdateType
		{
			FixedUpdate,
			LateUpdate
		}

		[Header("Base Settings")]
		public AfsDisplacementRenderer.UpdateType m_UpdateType;

		[SerializeField]
		private float m_GroundCheckDistance = 1f;

		[Header("Trail Settings"), SerializeField]
		private float m_TrailWidth = 1f;

		[SerializeField]
		private float m_TrailMaxTime = 1f;

		[Header("Force Settings")]
		public float m_ImpulseBias = 6f;

		public float m_MaxIdleImpulse = 1f;

		[Header("Outputs [Debugging only]")]
		public float m_Speed;

		public float m_Impulse;

		public float m_outputImpulse;

		public float m_outputIdleImpulse;

		internal Vector3 lastPosition = Vector3.zero;

		internal Vector3 currentPosition;

		public float m_DistanceToGround;

		private float m_TrailTime;

		private float velocity_speed;

		private float velocity_time;

		private float velocity_standspeed;

		private float smoothTime = 0.35f;

		private float smoothTimeIdle = 0.25f;

		public bool m_IsGrounded;

		private TrailRenderer m_TrailRenderer;

		public Material m_TrailRendererMat;

		private MeshRenderer m_MeshRenderer;

		public Material m_MeshRendererMat;

		private void Start()
		{
			this.m_TrailRenderer = base.GetComponent<TrailRenderer>();
			this.m_TrailRendererMat = this.m_TrailRenderer.material;
			this.m_TrailRenderer.startWidth = this.m_TrailWidth;
			this.m_TrailRenderer.endWidth = this.m_TrailWidth;
			this.m_MeshRenderer = base.GetComponent<MeshRenderer>();
			if (this.m_MeshRenderer)
			{
				this.m_MeshRendererMat = this.m_MeshRenderer.material;
			}
		}

		private void LateUpdate()
		{
			this.UpdateSpeed();
			this.CheckDistanceToGround();
		}

		private void UpdateSpeed()
		{
			this.currentPosition = base.transform.position;
			Vector2 a = new Vector2(this.currentPosition.x, this.currentPosition.z);
			Vector2 b = new Vector2(this.lastPosition.x, this.lastPosition.z);
			this.m_Speed = (a - b).magnitude / Time.deltaTime * 3.6f;
			this.lastPosition = this.currentPosition;
		}

		private void Update()
		{
			this.m_Impulse = this.m_Speed;
			this.m_outputImpulse = Mathf.SmoothDamp(this.m_outputImpulse, this.m_Impulse / this.m_ImpulseBias * (1f - this.m_DistanceToGround), ref this.velocity_speed, this.smoothTime);
			this.m_outputImpulse = Mathf.Clamp01(this.m_outputImpulse);
			if (this.m_outputImpulse <= 0.01f)
			{
				this.m_outputImpulse = 0f;
			}
			this.m_TrailRendererMat.SetFloat("_OverallPower", this.m_outputImpulse);
			this.m_TrailTime = Mathf.SmoothDamp(this.m_TrailTime, this.m_TrailMaxTime * 0.5f + (1f - this.m_DistanceToGround) * 0.5f, ref this.velocity_time, this.smoothTime);
			this.m_TrailRenderer.time = this.m_TrailTime;
			this.m_outputIdleImpulse = Mathf.SmoothDamp(this.m_outputIdleImpulse, (1f - this.m_outputImpulse) * (1f - this.m_DistanceToGround) * this.m_MaxIdleImpulse, ref this.velocity_standspeed, this.smoothTimeIdle);
			if (this.m_MeshRendererMat)
			{
				this.m_MeshRendererMat.SetFloat("_OverallPower", this.m_outputIdleImpulse);
			}
		}

		private void CheckDistanceToGround()
		{
			Terrain activeTerrain = Terrain.activeTerrain;
			if (activeTerrain)
			{
				float num = Mathf.Abs(activeTerrain.SampleHeight(base.transform.position) - base.transform.position.y);
				if (num < this.m_GroundCheckDistance)
				{
					this.m_DistanceToGround = num;
					this.m_IsGrounded = true;
				}
				else
				{
					this.m_IsGrounded = false;
					this.m_DistanceToGround = 1f;
				}
			}
		}
	}
}
