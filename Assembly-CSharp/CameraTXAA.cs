using System;
using TheForest.Utils;
using UnityEngine;

public class CameraTXAA : MonoBehaviour
{
	private Vector3 vp;

	private Vector3 vc;

	private Quaternion qp;

	private Quaternion qc;

	private void Start()
	{
		this.vp = (this.vc = base.transform.position);
		this.qp = (this.qc = base.transform.rotation);
	}

	private void Update()
	{
		if (LocalPlayer.Inventory.enabled)
		{
			base.transform.position = Vector3.Lerp(this.vp, this.vc, Mathf.Clamp01((Time.time - Time.fixedTime) / Time.fixedDeltaTime));
			base.transform.rotation = Quaternion.Lerp(this.qp, this.qc, Mathf.Clamp01((Time.time - Time.fixedTime) / Time.fixedDeltaTime));
		}
	}

	private void FixedUpdate()
	{
		this.vp = this.vc;
		this.vc = base.transform.position;
		this.qp = this.qc;
		this.qc = base.transform.rotation;
	}
}
