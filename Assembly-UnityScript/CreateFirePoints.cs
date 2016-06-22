using System;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
public class CreateFirePoints : MonoBehaviour
{
	public Transform FireDot;

	private int count;

	private bool warning;

	private string TString;

	private int createRatio;

	private int looper;

	public CreateFirePoints()
	{
		this.TString = string.Empty;
		this.looper = 1;
	}

	public override void Create()
	{
		if ((MeshFilter)this.gameObject.GetComponent(typeof(MeshFilter)))
		{
			if (!this.transform.Find("FirePoints"))
			{
				this.count = 0;
				Mesh sharedMesh = ((MeshFilter)this.GetComponent(typeof(MeshFilter))).sharedMesh;
				if (sharedMesh.vertexCount <= 2000 || this.warning)
				{
					if (int.TryParse(this.TString, out this.createRatio))
					{
						GameObject gameObject = new GameObject("FirePoints");
						gameObject.transform.parent = this.transform;
						gameObject.transform.position = this.transform.position;
						int i = 0;
						Vector3[] vertices = sharedMesh.vertices;
						int length = vertices.Length;
						while (i < length)
						{
							if (this.looper == 1)
							{
								Transform transform = (Transform)UnityEngine.Object.Instantiate(this.FireDot, this.transform.TransformPoint(vertices[i]), Quaternion.identity);
								transform.name = "FirePoint" + this.count;
								transform.parent = gameObject.transform;
								this.count++;
							}
							if (this.looper >= this.createRatio)
							{
								this.looper = 0;
							}
							this.looper++;
							i++;
						}
						Debug.Log(this.count + " Fire Points created.");
						this.warning = false;
					}
					else
					{
						Debug.LogError("createRatio is not of type int.");
					}
				}
				else
				{
					Debug.LogWarning("Vertices of the mesh exceeds 2000 (" + sharedMesh.vertexCount + "). It is recommended that you set the Create Ratio greater than 5.");
					this.warning = true;
				}
			}
			else
			{
				Debug.LogError("Fire Points already created.");
			}
		}
		else
		{
			Debug.LogError("No mesh present for the object.");
		}
	}

	public override void DestroyP()
	{
		if (this.transform.Find("FirePoints"))
		{
			UnityEngine.Object.DestroyImmediate(this.transform.Find("FirePoints").gameObject);
			Debug.Log("Fire Points destroyed.");
		}
	}

	public override void Main()
	{
	}
}
