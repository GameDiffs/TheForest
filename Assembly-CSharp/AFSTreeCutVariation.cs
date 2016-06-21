using System;
using UnityEngine;

public class AFSTreeCutVariation : MonoBehaviour
{
	[Space(5f)]
	public float BendingMultiplier = 10f;

	public float maxBendingStrength = 10f;

	[Space(5f)]
	public float TumblingMultiplier = 400f;

	public float maxTumblingStrength = 100f;

	[Range(0.1f, 4f)]
	public float FrequencyMultiplier = 1.5f;

	[Space(5f)]
	public float smoothTime = 0.35f;

	[Header("Debug"), Space(10f)]
	public float finalBendingStrength;

	public float finalTumblingStrength;

	private MaterialPropertyBlock CutVersionBlock;

	private Matrix4x4 m_Matrix;

	private Renderer m_renderer;

	private Rigidbody m_rb;

	private Vector3 smoothVelocity = Vector3.zero;

	private Vector3 smoothAngularVelocity = Vector3.zero;

	private float smoothTumbleFrequency;

	private float TumbleFrequency;

	private float t_TumbleFrequency;

	private Vector3 velocity;

	private Vector3 angularVelocity;

	private Vector3 n_velocity;

	private Vector3 n_angularVelocity;

	private Vector4 n_fallingWindVec;

	private void Awake()
	{
		this.m_renderer = base.GetComponent<Renderer>();
		this.m_rb = base.GetComponent<Rigidbody>();
		if (!this.m_rb)
		{
			this.m_rb = base.transform.root.GetComponent<Rigidbody>();
		}
		this.CutVersionBlock = new MaterialPropertyBlock();
		this.setProperty();
	}

	private void FixedUpdate()
	{
		this.velocity = Vector3.SmoothDamp(this.velocity, this.m_rb.velocity, ref this.smoothVelocity, this.smoothTime);
		this.angularVelocity = Vector3.SmoothDamp(this.angularVelocity, this.m_rb.angularVelocity, ref this.smoothAngularVelocity, this.smoothTime);
		this.finalBendingStrength = Mathf.Clamp(this.angularVelocity.magnitude * this.BendingMultiplier, 0f, this.maxBendingStrength);
		this.finalTumblingStrength = Mathf.Clamp(this.angularVelocity.magnitude * this.TumblingMultiplier, 0f, this.maxTumblingStrength);
		this.t_TumbleFrequency = Mathf.Lerp(1f, this.FrequencyMultiplier, this.finalBendingStrength / this.maxBendingStrength);
		this.TumbleFrequency = Mathf.SmoothDamp(this.TumbleFrequency, this.t_TumbleFrequency, ref this.smoothTumbleFrequency, this.smoothTime);
		this.n_fallingWindVec = new Vector4(0f, this.finalTumblingStrength / base.transform.lossyScale.y, this.finalBendingStrength / base.transform.lossyScale.y, this.TumbleFrequency);
		this.n_angularVelocity = this.angularVelocity;
		this.n_angularVelocity.Normalize();
		this.n_velocity = this.velocity;
		this.n_velocity.Normalize();
		this.m_renderer.GetPropertyBlock(this.CutVersionBlock);
		this.CutVersionBlock.SetVector("_FallingDir", this.n_velocity);
		this.CutVersionBlock.SetVector("_FallingWind", this.n_fallingWindVec);
		this.CutVersionBlock.SetVector("_FallingRotDir", this.n_angularVelocity);
		this.m_renderer.SetPropertyBlock(this.CutVersionBlock);
	}

	private void setProperty()
	{
		this.m_renderer.GetPropertyBlock(this.CutVersionBlock);
		this.CutVersionBlock.SetFloat("_TreeBendingMode", 1f);
		this.CutVersionBlock.SetFloat("_Variation", Mathf.Abs(Mathf.Abs(base.transform.position.x + base.transform.position.z) * 0.1f % 1f - 0.5f) * 2f);
		Quaternion q = Quaternion.AngleAxis(-base.transform.rotation.eulerAngles.y, Vector3.up);
		this.m_Matrix.SetTRS(Vector3.zero, q, new Vector3(1f, 1f, 1f));
		this.CutVersionBlock.SetMatrix("_TreeRotMatrix", this.m_Matrix);
		this.m_renderer.SetPropertyBlock(this.CutVersionBlock);
	}
}
