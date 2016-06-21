using Bolt;
using System;
using UnityEngine;

[ExecuteInEditMode]
public class BoltAOI : EntityBehaviour
{
	[SerializeField]
	public float detectRadius = 32f;

	[SerializeField]
	public float releaseRadius = 64f;

	[SerializeField]
	public int updateRate = 30;

	private void Update()
	{
		Graphics.DrawMesh(BoltPOI.Mesh, Matrix4x4.TRS(base.transform.position, Quaternion.identity, new Vector3(this.detectRadius, this.detectRadius, this.detectRadius)), BoltPOI.MaterialAOIDetect, 0);
		Graphics.DrawMesh(BoltPOI.Mesh, Matrix4x4.TRS(base.transform.position, Quaternion.identity, new Vector3(this.releaseRadius, this.releaseRadius, this.releaseRadius)), BoltPOI.MaterialAOIRelease, 0);
	}

	public override void SimulateOwner()
	{
		if (BoltNetwork.frame % 30 == 0 && BoltNetwork.scopeMode == ScopeMode.Manual && base.enabled && this.entity.controller != null)
		{
			BoltPOI.UpdateScope(this, this.entity.controller);
		}
	}
}
