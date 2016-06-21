using Bolt;
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoltPOI : EntityBehaviour
{
	private static Mesh mesh;

	private static Material poiMat;

	private static Material aoiDetectMat;

	private static Material aoiReleaseMat;

	private static List<BoltPOI> availablePOIs = new List<BoltPOI>();

	[SerializeField]
	public float radius = 16f;

	public static Mesh Mesh
	{
		get
		{
			if (!BoltPOI.mesh)
			{
				BoltPOI.mesh = (Mesh)Resources.Load("IcoSphere", typeof(Mesh));
			}
			return BoltPOI.mesh;
		}
	}

	public static Material MaterialPOI
	{
		get
		{
			if (!BoltPOI.poiMat)
			{
				BoltPOI.poiMat = BoltPOI.CreateMaterial(new Color(0f, 0.145098045f, 1f));
			}
			return BoltPOI.poiMat;
		}
	}

	public static Material MaterialAOIDetect
	{
		get
		{
			if (!BoltPOI.aoiDetectMat)
			{
				BoltPOI.aoiDetectMat = BoltPOI.CreateMaterial(new Color(0.145098045f, 0.4f, 0f));
			}
			return BoltPOI.aoiDetectMat;
		}
	}

	public static Material MaterialAOIRelease
	{
		get
		{
			if (!BoltPOI.aoiReleaseMat)
			{
				BoltPOI.aoiReleaseMat = BoltPOI.CreateMaterial(new Color(1f, 0.145098045f, 0f));
			}
			return BoltPOI.aoiReleaseMat;
		}
	}

	private static Material CreateMaterial(Color c)
	{
		Material material = new Material(Resources.Load("BoltShaderPOI", typeof(Shader)) as Shader);
		material.hideFlags = HideFlags.HideAndDontSave;
		material.SetColor("_SpecColor", c);
		return material;
	}

	private void Update()
	{
		Graphics.DrawMesh(BoltPOI.Mesh, Matrix4x4.TRS(base.transform.position, Quaternion.identity, new Vector3(this.radius, this.radius, this.radius)), BoltPOI.MaterialPOI, 0);
	}

	private void OnDestroy()
	{
		BoltPOI.availablePOIs.Remove(this);
	}

	private void BoltSceneObject()
	{
		if (BoltNetwork.isClient)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public override void Attached()
	{
		BoltPOI.availablePOIs.Add(this);
	}

	public override void Detached()
	{
		BoltPOI.availablePOIs.Remove(this);
	}

	public static void UpdateScope(BoltAOI aoi, BoltConnection connection)
	{
		Vector3 position = aoi.transform.position;
		float detectRadius = aoi.detectRadius;
		float releaseRadius = aoi.releaseRadius;
		for (int i = 0; i < BoltPOI.availablePOIs.Count; i++)
		{
			BoltPOI boltPOI = BoltPOI.availablePOIs[i];
			Vector3 position2 = boltPOI.transform.position;
			float bRadius = boltPOI.radius;
			if (BoltPOI.OverlapSphere(position, position2, detectRadius, bRadius))
			{
				boltPOI.entity.SetScope(connection, true);
			}
			else if (!BoltPOI.OverlapSphere(position, position2, releaseRadius, bRadius))
			{
				boltPOI.entity.SetScope(connection, false);
			}
		}
	}

	private static bool OverlapSphere(Vector3 a, Vector3 b, float aRadius, float bRadius)
	{
		float num = aRadius + bRadius;
		return (a - b).sqrMagnitude <= num * num;
	}
}
