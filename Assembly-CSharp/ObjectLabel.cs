using System;
using UnityEngine;

[RequireComponent(typeof(GUIText))]
public class ObjectLabel : MonoBehaviour
{
	public Transform target;

	public Vector3 offset = Vector3.up;

	public bool clampToScreen;

	public float clampBorderSize = 0.05f;

	public bool useMainCamera = true;

	public Camera cameraToUse;

	private Camera cam;

	private Transform thisTransform;

	private Transform camTransform;

	private GUIText label;

	private EnemyHealth mutantHealth;

	private void Awake()
	{
		Debug.Log("great");
		this.label = base.GetComponent<GUIText>();
		this.mutantHealth = this.target.GetComponent<EnemyHealth>();
		this.thisTransform = base.transform;
		if (this.useMainCamera)
		{
			this.cam = Camera.main;
		}
		else
		{
			this.cam = this.cameraToUse;
		}
		this.camTransform = this.cam.transform;
	}

	private void Update()
	{
		int health = this.mutantHealth.Health;
		this.label.text = health.ToString();
		if (this.clampToScreen)
		{
			Vector3 a = this.camTransform.InverseTransformPoint(this.target.position);
			a.z = Mathf.Max(a.z, 1f);
			this.thisTransform.position = this.cam.WorldToViewportPoint(this.camTransform.TransformPoint(a + this.offset));
			this.thisTransform.position = new Vector3(Mathf.Clamp(this.thisTransform.position.x, this.clampBorderSize, 1f - this.clampBorderSize), Mathf.Clamp(this.thisTransform.position.y, this.clampBorderSize, 1f - this.clampBorderSize), this.thisTransform.position.z);
		}
		else
		{
			this.thisTransform.position = this.cam.WorldToViewportPoint(this.target.position + this.offset);
		}
	}
}
