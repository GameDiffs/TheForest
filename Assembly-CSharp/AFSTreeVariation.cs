using System;
using UnityEngine;

public class AFSTreeVariation : MonoBehaviour
{
	public bool isCut;

	public float smoothTime = 0.5f;

	public Vector3 velocity;

	private Matrix4x4 m_Matrix;

	private Renderer renderer;

	private Rigidbody rb;

	private Material mat;

	private Vector3 smoothVelocity = Vector3.zero;

	private void Start()
	{
		this.renderer = base.GetComponent<Renderer>();
		this.rb = base.GetComponent<Rigidbody>();
		this.mat = this.renderer.material;
		this.InitShaderProperties();
	}

	private void InitShaderProperties()
	{
		if (this.isCut)
		{
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			this.renderer.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetFloat("_Variation", Mathf.Abs(Mathf.Abs(base.transform.position.x + base.transform.position.z) * 0.1f % 1f - 0.5f) * 2f);
			Quaternion q = Quaternion.AngleAxis(-base.transform.rotation.eulerAngles.y, Vector3.up);
			this.m_Matrix.SetTRS(Vector3.zero, q, new Vector3(1f, 1f, 1f));
			materialPropertyBlock.SetMatrix("_TreeRotMatrix", this.m_Matrix);
			this.renderer.SetPropertyBlock(materialPropertyBlock);
			this.mat.EnableKeyword("_CUTVERSION");
		}
		else
		{
			this.mat.DisableKeyword("_CUTVERSION");
		}
	}
}
