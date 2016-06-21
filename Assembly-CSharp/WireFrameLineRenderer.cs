using System;
using System.Collections.Generic;
using UnityEngine;

public class WireFrameLineRenderer : MonoBehaviour
{
	public class Line
	{
		public Vector3 PointA;

		public Vector3 PointB;

		public Line(Vector3 a, Vector3 b)
		{
			this.PointA = a;
			this.PointB = b;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			WireFrameLineRenderer.Line line = obj as WireFrameLineRenderer.Line;
			return line != null && ((this.PointA == line.PointA && this.PointB == line.PointB) || (this.PointA == line.PointB && this.PointB == line.PointA));
		}

		public bool Equals(WireFrameLineRenderer.Line lB)
		{
			return (this.PointA == lB.PointA && this.PointB == lB.PointB) || (this.PointA == lB.PointB && this.PointB == lB.PointA);
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public static bool operator ==(WireFrameLineRenderer.Line lA, WireFrameLineRenderer.Line lB)
		{
			return (lA.PointA == lB.PointA && lA.PointB == lB.PointB) || (lA.PointA == lB.PointB && lA.PointB == lB.PointA);
		}

		public static bool operator !=(WireFrameLineRenderer.Line lA, WireFrameLineRenderer.Line lB)
		{
			return !(lA == lB);
		}
	}

	public Color LineColor;

	public bool ZWrite = true;

	public bool AWrite = true;

	public bool Blend = true;

	public int Fidelity = 3;

	private Vector3[] Lines;

	private List<WireFrameLineRenderer.Line> LinesArray = new List<WireFrameLineRenderer.Line>();

	private Material LineMaterial;

	public void Start()
	{
		this.LineMaterial = new Material("Shader \"Lines/Colored Blended\" { SubShader { Pass { Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Front Fog { Mode Off } } } }");
		this.LineMaterial.hideFlags = HideFlags.HideAndDontSave;
		this.LineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		MeshFilter component = base.GetComponent<MeshFilter>();
		Mesh sharedMesh = component.sharedMesh;
		Vector3[] vertices = sharedMesh.vertices;
		int[] triangles = sharedMesh.triangles;
		for (int i = 0; i < triangles.Length / 3; i++)
		{
			int num = i * 3;
			WireFrameLineRenderer.Line l = new WireFrameLineRenderer.Line(vertices[triangles[num]], vertices[triangles[num + 1]]);
			WireFrameLineRenderer.Line l2 = new WireFrameLineRenderer.Line(vertices[triangles[num + 1]], vertices[triangles[num + 2]]);
			WireFrameLineRenderer.Line l3 = new WireFrameLineRenderer.Line(vertices[triangles[num + 2]], vertices[triangles[num]]);
			if (this.Fidelity == 3)
			{
				this.AddLine(l);
				this.AddLine(l2);
				this.AddLine(l3);
			}
			else if (this.Fidelity == 2)
			{
				this.AddLine(l);
				this.AddLine(l2);
			}
			else if (this.Fidelity == 1)
			{
				this.AddLine(l);
			}
		}
	}

	public void AddLine(WireFrameLineRenderer.Line l)
	{
		bool flag = false;
		foreach (WireFrameLineRenderer.Line current in this.LinesArray)
		{
			if (l == current)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this.LinesArray.Add(l);
		}
	}

	public void OnRenderObject()
	{
		this.LineMaterial.SetPass(0);
		GL.PushMatrix();
		GL.MultMatrix(base.transform.localToWorldMatrix);
		GL.Begin(1);
		GL.Color(this.LineColor);
		foreach (WireFrameLineRenderer.Line current in this.LinesArray)
		{
			GL.Vertex(current.PointA);
			GL.Vertex(current.PointB);
		}
		GL.End();
		GL.PopMatrix();
	}
}
