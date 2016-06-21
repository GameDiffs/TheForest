using System;
using UnityEngine;

[ExecuteInEditMode]
public class AfsBillboardBoundingBox_180 : MonoBehaviour
{
	public bool instatiated;

	private float TreeHeight;

	private float TreeScale;

	public float FadeNearDistance = 20f;

	public float FadeFarDistance = 21f;

	public float FarKillDistance = 16384f;

	[Range(0f, 179.9f)]
	public float Rotation;

	private void Awake()
	{
		this.SetBounds();
		this.SetStippling();
	}

	private void CheckInstance()
	{
		if (!this.instatiated)
		{
			Mesh sharedMesh = base.GetComponent<MeshFilter>().sharedMesh;
			Mesh sharedMesh2 = UnityEngine.Object.Instantiate<Mesh>(sharedMesh);
			base.GetComponent<MeshFilter>().sharedMesh = sharedMesh2;
			this.instatiated = true;
		}
	}

	private void SetBounds()
	{
		Mesh sharedMesh = base.GetComponent<MeshFilter>().sharedMesh;
		Material sharedMaterial = base.GetComponent<MeshRenderer>().sharedMaterial;
		if (sharedMaterial.HasProperty("_TreeHeight"))
		{
			this.TreeHeight = sharedMaterial.GetFloat("_TreeHeight");
			this.TreeScale = sharedMaterial.GetFloat("_TreeScale");
			Bounds bounds = new Bounds(new Vector3(0f, this.TreeScale * this.TreeHeight * 0.5f, 0f), new Vector3(this.TreeScale, this.TreeScale * this.TreeHeight, this.TreeScale));
			sharedMesh.bounds = bounds;
		}
	}

	private void SetRotation()
	{
		Mesh sharedMesh = base.GetComponent<MeshFilter>().sharedMesh;
		Vector3[] vertices = sharedMesh.vertices;
		Color[] array = sharedMesh.colors;
		if (array.Length == 0)
		{
			Debug.Log("Vertex Colors added.");
			array = new Color[vertices.Length];
		}
		Quaternion rotation = Quaternion.AngleAxis(this.Rotation, Vector3.up);
		Vector3 vector = rotation * Vector3.forward;
		float num = Mathf.Atan2(vector.x, vector.z);
		num = (num + 3.14159274f) / 6.28318548f;
		for (int i = 0; i < vertices.Length; i++)
		{
			array[i] = new Color(0f, 0f, num, 0f);
		}
		sharedMesh.colors = array;
	}

	private void SetStippling()
	{
		base.GetComponent<Renderer>().sharedMaterial.SetVector("_StippleRangeSq", new Vector4(this.FadeNearDistance * this.FadeNearDistance, this.FadeFarDistance * this.FadeFarDistance, 0f, this.FarKillDistance * this.FarKillDistance));
	}
}
