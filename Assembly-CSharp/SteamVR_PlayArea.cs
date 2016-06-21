using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;
using Valve.VR;

[ExecuteInEditMode, RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class SteamVR_PlayArea : MonoBehaviour
{
	public enum Size
	{
		Calibrated,
		_400x300,
		_300x225,
		_200x150
	}

	public float borderThickness = 0.15f;

	public float wireframeHeight = 2f;

	public bool drawWireframeWhenSelectedOnly;

	public bool drawInGame = true;

	public SteamVR_PlayArea.Size size;

	public Color color = Color.cyan;

	[HideInInspector]
	public Vector3[] vertices;

	public static bool GetBounds(SteamVR_PlayArea.Size size, ref HmdQuad_t pRect)
	{
		if (size == SteamVR_PlayArea.Size.Calibrated)
		{
			bool flag = !SteamVR.active && !SteamVR.usingNativeSupport;
			if (flag)
			{
				EVRInitError eVRInitError = EVRInitError.None;
				OpenVR.Init(ref eVRInitError, EVRApplicationType.VRApplication_Other);
			}
			CVRChaperone chaperone = OpenVR.Chaperone;
			bool flag2 = chaperone != null && chaperone.GetPlayAreaRect(ref pRect);
			if (!flag2)
			{
				UnityEngine.Debug.LogWarning("Failed to get Calibrated Play Area bounds!  Make sure you have tracking first, and that your space is calibrated.");
			}
			if (flag)
			{
				OpenVR.Shutdown();
			}
			return flag2;
		}
		try
		{
			string text = size.ToString().Substring(1);
			string[] array = text.Split(new char[]
			{
				'x'
			}, 2);
			float num = float.Parse(array[0]) / 200f;
			float num2 = float.Parse(array[1]) / 200f;
			pRect.vCorners0.v0 = num;
			pRect.vCorners0.v1 = 0f;
			pRect.vCorners0.v2 = num2;
			pRect.vCorners1.v0 = num;
			pRect.vCorners1.v1 = 0f;
			pRect.vCorners1.v2 = -num2;
			pRect.vCorners2.v0 = -num;
			pRect.vCorners2.v1 = 0f;
			pRect.vCorners2.v2 = -num2;
			pRect.vCorners3.v0 = -num;
			pRect.vCorners3.v1 = 0f;
			pRect.vCorners3.v2 = num2;
			return true;
		}
		catch
		{
		}
		return false;
	}

	public void BuildMesh()
	{
		HmdQuad_t hmdQuad_t = default(HmdQuad_t);
		if (!SteamVR_PlayArea.GetBounds(this.size, ref hmdQuad_t))
		{
			return;
		}
		HmdVector3_t[] array = new HmdVector3_t[]
		{
			hmdQuad_t.vCorners0,
			hmdQuad_t.vCorners1,
			hmdQuad_t.vCorners2,
			hmdQuad_t.vCorners3
		};
		this.vertices = new Vector3[array.Length * 2];
		for (int i = 0; i < array.Length; i++)
		{
			HmdVector3_t hmdVector3_t = array[i];
			this.vertices[i] = new Vector3(hmdVector3_t.v0, 0.01f, hmdVector3_t.v2);
		}
		if (this.borderThickness == 0f)
		{
			base.GetComponent<MeshFilter>().mesh = null;
			return;
		}
		for (int j = 0; j < array.Length; j++)
		{
			int num = (j + 1) % array.Length;
			int num2 = (j + array.Length - 1) % array.Length;
			Vector3 normalized = (this.vertices[num] - this.vertices[j]).normalized;
			Vector3 normalized2 = (this.vertices[num2] - this.vertices[j]).normalized;
			Vector3 vector = this.vertices[j];
			vector += Vector3.Cross(normalized, Vector3.up) * this.borderThickness;
			vector += Vector3.Cross(normalized2, Vector3.down) * this.borderThickness;
			this.vertices[array.Length + j] = vector;
		}
		int[] triangles = new int[]
		{
			0,
			1,
			4,
			1,
			5,
			4,
			1,
			2,
			5,
			2,
			6,
			5,
			2,
			3,
			6,
			3,
			7,
			6,
			3,
			0,
			7,
			0,
			4,
			7
		};
		Vector2[] uv = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		Color[] colors = new Color[]
		{
			this.color,
			this.color,
			this.color,
			this.color,
			new Color(this.color.r, this.color.g, this.color.b, 0f),
			new Color(this.color.r, this.color.g, this.color.b, 0f),
			new Color(this.color.r, this.color.g, this.color.b, 0f),
			new Color(this.color.r, this.color.g, this.color.b, 0f)
		};
		Mesh mesh = new Mesh();
		base.GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = this.vertices;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.triangles = triangles;
		MeshRenderer component = base.GetComponent<MeshRenderer>();
		component.material = Resources.GetBuiltinResource<Material>("Sprites-Default.mat");
		component.reflectionProbeUsage = ReflectionProbeUsage.Off;
		component.shadowCastingMode = ShadowCastingMode.Off;
		component.receiveShadows = false;
		component.useLightProbes = false;
	}

	private void OnDrawGizmos()
	{
		if (!this.drawWireframeWhenSelectedOnly)
		{
			this.DrawWireframe();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (this.drawWireframeWhenSelectedOnly)
		{
			this.DrawWireframe();
		}
	}

	public void DrawWireframe()
	{
		if (this.vertices == null || this.vertices.Length == 0)
		{
			return;
		}
		Vector3 b = base.transform.TransformVector(Vector3.up * this.wireframeHeight);
		for (int i = 0; i < 4; i++)
		{
			int num = (i + 1) % 4;
			Vector3 vector = base.transform.TransformPoint(this.vertices[i]);
			Vector3 vector2 = vector + b;
			Vector3 vector3 = base.transform.TransformPoint(this.vertices[num]);
			Vector3 to = vector3 + b;
			Gizmos.DrawLine(vector, vector2);
			Gizmos.DrawLine(vector, vector3);
			Gizmos.DrawLine(vector2, to);
		}
	}

	public void OnEnable()
	{
		if (Application.isPlaying)
		{
			base.GetComponent<MeshRenderer>().enabled = this.drawInGame;
			base.enabled = false;
			if (this.drawInGame && this.size == SteamVR_PlayArea.Size.Calibrated)
			{
				base.StartCoroutine("UpdateBounds");
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator UpdateBounds()
	{
		SteamVR_PlayArea.<UpdateBounds>c__Iterator1D6 <UpdateBounds>c__Iterator1D = new SteamVR_PlayArea.<UpdateBounds>c__Iterator1D6();
		<UpdateBounds>c__Iterator1D.<>f__this = this;
		return <UpdateBounds>c__Iterator1D;
	}
}
