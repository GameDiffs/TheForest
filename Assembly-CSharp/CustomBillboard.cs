using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

[ExecuteInEditMode]
public class CustomBillboard : MonoBehaviour
{
	private const float kOneOver180 = 0.00555555569f;

	public float FadeNearDistance = 20f;

	public float FadeFarDistance = 21f;

	public float FarKillDistance = 16384f;

	public bool UseLowFarKillDistanceQuality;

	public int Quantity = 10;

	[Header("Horizontal Billboards")]
	public bool UsesHorizontal;

	public float HorizontalOffset = 20f;

	public float HorizontalSize = 3f;

	private Mesh mesh;

	private Vector3 hOffset0;

	private Vector3 hOffset1;

	private Vector3 hOffset2;

	private Vector3 hOffset3;

	private readonly Vector2 uv0 = new Vector2(0f, 0f);

	private readonly Vector2 uv1 = new Vector2(0f, 1f);

	private readonly Vector2 uv2 = new Vector2(1f, 0f);

	private readonly Vector2 uv3 = new Vector2(1f, 1f);

	private Vector3[] vertices;

	private Color[] colors;

	private List<bool> registeredAlive = new List<bool>();

	private List<Vector3> registeredPositions = new List<Vector3>();

	private List<float> registeredRotations = new List<float>();

	private int BuiltBillboardCount;

	private bool positionChange;

	private bool aliveChange;

	private bool usingHorizontal;

	private bool ShouldUseHorizontal
	{
		get
		{
			return this.UsesHorizontal && TheForestQualitySettings.UserSettings.FarShadowMode == TheForestQualitySettings.FarShadowModes.On;
		}
	}

	private void Awake()
	{
		if (Application.isPlaying && (Application.platform == RuntimePlatform.PS4 || PlayerPreferences.is32bit))
		{
			this.UsesHorizontal = false;
		}
		if (this.UsesHorizontal)
		{
			this.hOffset0 = this.HorizontalSize * Vector3.forward;
			this.hOffset1 = this.HorizontalSize * Vector3.right;
			this.hOffset2 = this.HorizontalSize * Vector3.left;
			this.hOffset3 = this.HorizontalSize * Vector3.back;
		}
		base.transform.rotation = Quaternion.identity;
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (component == null)
		{
			Debug.LogError("MeshFilter not found!");
			return;
		}
		this.mesh = new Mesh();
		this.mesh.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild);
		this.mesh.hideFlags ^= (this.mesh.hideFlags & HideFlags.DontUnloadUnusedAsset);
		this.mesh.name = "CustomBillboard-" + base.name;
		component.sharedMesh = this.mesh;
	}

	private void LateUpdate()
	{
		if (this.BuiltBillboardCount != this.registeredPositions.Count || this.usingHorizontal != this.ShouldUseHorizontal)
		{
			this.BuildMesh();
		}
		if (this.positionChange)
		{
			this.UpdatePositions();
			this.positionChange = false;
		}
		if (this.aliveChange)
		{
			this.UpdateAlive();
			this.aliveChange = false;
		}
		if (this.UseLowFarKillDistanceQuality && TheForestQualitySettings.UserSettings.DrawDistance >= TheForestQualitySettings.DrawDistances.Low)
		{
			base.GetComponent<Renderer>().sharedMaterial.SetVector("_StippleRangeSq", new Vector4(this.FadeNearDistance * this.FadeNearDistance, this.FadeFarDistance * this.FadeFarDistance, 0f, this.FarKillDistance * this.FarKillDistance / 4f));
		}
		else
		{
			base.GetComponent<Renderer>().sharedMaterial.SetVector("_StippleRangeSq", new Vector4(this.FadeNearDistance * this.FadeNearDistance, this.FadeFarDistance * this.FadeFarDistance, 0f, this.FarKillDistance * this.FarKillDistance));
		}
		if (LocalPlayer.MainCamTr)
		{
			base.GetComponent<Renderer>().sharedMaterial.SetVector("_CameraRight", LocalPlayer.MainCamTr.right);
		}
	}

	private void OnDestroy()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (component)
		{
			component.sharedMesh = null;
		}
		if (this.mesh)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(this.mesh);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(this.mesh);
			}
			this.mesh = null;
		}
	}

	public int Register(Vector3 position, float yRotation)
	{
		this.registeredPositions.Add(position);
		this.registeredRotations.Add(this.GetRotation(yRotation));
		this.registeredAlive.Add(true);
		return this.registeredPositions.Count - 1;
	}

	public void SetPosition(int index, Vector3 position)
	{
		if (this.registeredPositions[index] != position)
		{
			this.registeredPositions[index] = position;
			int startV = this.GetStartV(index);
			this.vertices[startV] = position;
			this.vertices[startV + 1] = position;
			this.vertices[startV + 2] = position;
			this.vertices[startV + 3] = position;
			this.positionChange = true;
		}
	}

	private int GetStartV(int index)
	{
		return index * 4;
	}

	private float GetRotation(float yRotation)
	{
		Quaternion rotation = Quaternion.AngleAxis(yRotation, Vector3.up);
		Vector3 vector = rotation * Vector3.forward;
		float num = Mathf.Atan2(vector.x, vector.z);
		return (num + 3.14159274f) / 6.28318548f;
	}

	public void SetAlive(int index, bool alive)
	{
		if (this.registeredAlive[index] != alive)
		{
			this.registeredAlive[index] = alive;
			if (this.colors != null && this.colors.Length > index)
			{
				int num = index * 4;
				Color color = new Color((!alive) ? 1f : 0f, 0f, this.registeredRotations[index], 0f);
				this.colors[num] = color;
				this.colors[num + 1] = color;
				this.colors[num + 2] = color;
				this.colors[num + 3] = color;
				this.aliveChange = true;
			}
		}
	}

	public void Show(int index)
	{
	}

	public void Hide(int index)
	{
	}

	public void Stippling(int index, Vector3 vec)
	{
	}

	private void BuildMesh()
	{
		this.usingHorizontal = this.ShouldUseHorizontal;
		int count = this.registeredPositions.Count;
		int num = ((!this.usingHorizontal) ? 4 : 8) * count;
		this.vertices = new Vector3[num];
		this.colors = new Color[num];
		Vector2[] array = new Vector2[num];
		int[] array2 = new int[((!this.usingHorizontal) ? 6 : 12) * count];
		int num2 = 4 * count;
		int num3 = 6 * count;
		int num4 = 4;
		int num5 = 6;
		for (int i = 0; i < count; i++)
		{
			int num6 = num4 * i;
			Vector3 vector = this.registeredPositions[i];
			this.vertices[num6] = vector;
			this.vertices[num6 + 1] = vector;
			this.vertices[num6 + 2] = vector;
			this.vertices[num6 + 3] = vector;
			Color color = new Color((!this.registeredAlive[i]) ? 1f : 0f, 0f, this.registeredRotations[i], 0f);
			this.colors[num6] = color;
			this.colors[num6 + 1] = color;
			this.colors[num6 + 2] = color;
			this.colors[num6 + 3] = color;
			array[num6] = this.uv0;
			array[num6 + 1] = this.uv1;
			array[num6 + 2] = this.uv2;
			array[num6 + 3] = this.uv3;
			int num7 = num5 * i;
			array2[num7] = 0 + num6;
			array2[num7 + 1] = 1 + num6;
			array2[num7 + 2] = 2 + num6;
			array2[num7 + 3] = 2 + num6;
			array2[num7 + 4] = 1 + num6;
			array2[num7 + 5] = 3 + num6;
			if (this.usingHorizontal)
			{
				num6 += num2;
				vector.y += this.HorizontalOffset;
				this.vertices[num6] = vector + this.hOffset0;
				this.vertices[num6 + 1] = vector + this.hOffset1;
				this.vertices[num6 + 2] = vector + this.hOffset2;
				this.vertices[num6 + 3] = vector + this.hOffset3;
				color = new Color((!this.registeredAlive[i]) ? 1f : 0f, 1f, this.registeredRotations[i], 0f);
				this.colors[num6] = color;
				this.colors[num6 + 1] = color;
				this.colors[num6 + 2] = color;
				this.colors[num6 + 3] = color;
				array[num6] = this.uv0;
				array[num6 + 1] = this.uv1;
				array[num6 + 2] = this.uv2;
				array[num6 + 3] = this.uv3;
				num7 += num3;
				array2[num7] = 0 + num6;
				array2[num7 + 1] = 1 + num6;
				array2[num7 + 2] = 2 + num6;
				array2[num7 + 3] = 2 + num6;
				array2[num7 + 4] = 1 + num6;
				array2[num7 + 5] = 3 + num6;
			}
		}
		this.mesh.Clear();
		this.mesh.vertices = this.vertices;
		this.mesh.colors = this.colors;
		this.mesh.uv = array;
		this.mesh.triangles = array2;
		this.mesh.normals = new Vector3[this.vertices.Length];
		this.mesh.bounds = new Bounds(Vector3.zero, new Vector3(10000f, 10000f, 10000f));
		this.mesh.Optimize();
		this.BuiltBillboardCount = this.registeredPositions.Count;
	}

	private void UpdatePositions()
	{
		this.mesh.vertices = this.vertices;
	}

	private void UpdateAlive()
	{
		this.mesh.colors = this.colors;
	}
}
