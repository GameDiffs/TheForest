using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Draw Call"), ExecuteInEditMode]
public class UIDrawCall : MonoBehaviour
{
	public enum Clipping
	{
		None,
		TextureMask,
		SoftClip = 3,
		ConstrainButDontClip
	}

	public delegate void OnRenderCallback(Material mat);

	private const int maxIndexBufferCache = 10;

	private static BetterList<UIDrawCall> mActiveList = new BetterList<UIDrawCall>();

	private static BetterList<UIDrawCall> mInactiveList = new BetterList<UIDrawCall>();

	[HideInInspector]
	[NonSerialized]
	public int widgetCount;

	[HideInInspector]
	[NonSerialized]
	public int depthStart = 2147483647;

	[HideInInspector]
	[NonSerialized]
	public int depthEnd = -2147483648;

	[HideInInspector]
	[NonSerialized]
	public UIPanel manager;

	[HideInInspector]
	[NonSerialized]
	public UIPanel panel;

	[HideInInspector]
	[NonSerialized]
	public Texture2D clipTexture;

	[HideInInspector]
	[NonSerialized]
	public bool alwaysOnScreen;

	[HideInInspector]
	[NonSerialized]
	public BetterList<Vector3> verts = new BetterList<Vector3>();

	[HideInInspector]
	[NonSerialized]
	public BetterList<Vector3> norms = new BetterList<Vector3>();

	[HideInInspector]
	[NonSerialized]
	public BetterList<Vector4> tans = new BetterList<Vector4>();

	[HideInInspector]
	[NonSerialized]
	public BetterList<Vector2> uvs = new BetterList<Vector2>();

	[HideInInspector]
	[NonSerialized]
	public BetterList<Color32> cols = new BetterList<Color32>();

	private Material mMaterial;

	private Texture mTexture;

	private Shader mShader;

	private int mClipCount;

	private Transform mTrans;

	private Mesh mMesh;

	private MeshFilter mFilter;

	private MeshRenderer mRenderer;

	private Material mDynamicMat;

	private int[] mIndices;

	private bool mRebuildMat = true;

	private bool mLegacyShader;

	private int mRenderQueue = 3000;

	private int mTriangles;

	[NonSerialized]
	public bool isDirty;

	[NonSerialized]
	private bool mTextureClip;

	public UIDrawCall.OnRenderCallback onRender;

	private static List<int[]> mCache = new List<int[]>(10);

	private static int[] ClipRange = null;

	private static int[] ClipArgs = null;

	[Obsolete("Use UIDrawCall.activeList")]
	public static BetterList<UIDrawCall> list
	{
		get
		{
			return UIDrawCall.mActiveList;
		}
	}

	public static BetterList<UIDrawCall> activeList
	{
		get
		{
			return UIDrawCall.mActiveList;
		}
	}

	public static BetterList<UIDrawCall> inactiveList
	{
		get
		{
			return UIDrawCall.mInactiveList;
		}
	}

	public int renderQueue
	{
		get
		{
			return this.mRenderQueue;
		}
		set
		{
			if (this.mRenderQueue != value)
			{
				this.mRenderQueue = value;
				if (this.mDynamicMat != null)
				{
					this.mDynamicMat.renderQueue = value;
				}
			}
		}
	}

	public int sortingOrder
	{
		get
		{
			return (!(this.mRenderer != null)) ? 0 : this.mRenderer.sortingOrder;
		}
		set
		{
			if (this.mRenderer != null && this.mRenderer.sortingOrder != value)
			{
				this.mRenderer.sortingOrder = value;
			}
		}
	}

	public int finalRenderQueue
	{
		get
		{
			return (!(this.mDynamicMat != null)) ? this.mRenderQueue : this.mDynamicMat.renderQueue;
		}
	}

	public Transform cachedTransform
	{
		get
		{
			if (this.mTrans == null)
			{
				this.mTrans = base.transform;
			}
			return this.mTrans;
		}
	}

	public Material baseMaterial
	{
		get
		{
			return this.mMaterial;
		}
		set
		{
			if (this.mMaterial != value)
			{
				this.mMaterial = value;
				this.mRebuildMat = true;
			}
		}
	}

	public Material dynamicMaterial
	{
		get
		{
			return this.mDynamicMat;
		}
	}

	public Texture mainTexture
	{
		get
		{
			return this.mTexture;
		}
		set
		{
			this.mTexture = value;
			if (this.mDynamicMat != null)
			{
				this.mDynamicMat.mainTexture = value;
			}
		}
	}

	public Shader shader
	{
		get
		{
			return this.mShader;
		}
		set
		{
			if (this.mShader != value)
			{
				this.mShader = value;
				this.mRebuildMat = true;
			}
		}
	}

	public int triangles
	{
		get
		{
			return (!(this.mMesh != null)) ? 0 : this.mTriangles;
		}
	}

	public bool isClipped
	{
		get
		{
			return this.mClipCount != 0;
		}
	}

	private void CreateMaterial()
	{
		this.mTextureClip = false;
		this.mLegacyShader = false;
		this.mClipCount = this.panel.clipCount;
		string text = (!(this.mShader != null)) ? ((!(this.mMaterial != null)) ? "Unlit/Transparent Colored" : this.mMaterial.shader.name) : this.mShader.name;
		text = text.Replace("GUI/Text Shader", "Unlit/Text");
		if (text.Length > 2 && text[text.Length - 2] == ' ')
		{
			int num = (int)text[text.Length - 1];
			if (num > 48 && num <= 57)
			{
				text = text.Substring(0, text.Length - 2);
			}
		}
		if (text.StartsWith("Hidden/"))
		{
			text = text.Substring(7);
		}
		text = text.Replace(" (SoftClip)", string.Empty);
		text = text.Replace(" (TextureClip)", string.Empty);
		if (this.panel.clipping == UIDrawCall.Clipping.TextureMask)
		{
			this.mTextureClip = true;
			this.shader = Shader.Find("Hidden/" + text + " (TextureClip)");
		}
		else if (this.mClipCount != 0)
		{
			this.shader = Shader.Find(string.Concat(new object[]
			{
				"Hidden/",
				text,
				" ",
				this.mClipCount
			}));
			if (this.shader == null)
			{
				this.shader = Shader.Find(text + " " + this.mClipCount);
			}
			if (this.shader == null && this.mClipCount == 1)
			{
				this.mLegacyShader = true;
				this.shader = Shader.Find(text + " (SoftClip)");
			}
		}
		else
		{
			this.shader = Shader.Find(text);
		}
		if (this.shader == null)
		{
			this.shader = Shader.Find("Unlit/Transparent Colored");
		}
		if (this.mMaterial != null)
		{
			this.mDynamicMat = new Material(this.mMaterial);
			this.mDynamicMat.name = "[NGUI] " + this.mMaterial.name;
			this.mDynamicMat.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontUnloadUnusedAsset | HideFlags.DontSaveInBuild);
			this.mDynamicMat.CopyPropertiesFromMaterial(this.mMaterial);
			string[] shaderKeywords = this.mMaterial.shaderKeywords;
			for (int i = 0; i < shaderKeywords.Length; i++)
			{
				this.mDynamicMat.EnableKeyword(shaderKeywords[i]);
			}
			if (this.shader != null)
			{
				this.mDynamicMat.shader = this.shader;
			}
			else if (this.mClipCount != 0)
			{
				Debug.LogError(string.Concat(new object[]
				{
					text,
					" shader doesn't have a clipped shader version for ",
					this.mClipCount,
					" clip regions"
				}));
			}
		}
		else
		{
			this.mDynamicMat = new Material(this.shader);
			this.mDynamicMat.name = "[NGUI] " + this.shader.name;
			this.mDynamicMat.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontUnloadUnusedAsset | HideFlags.DontSaveInBuild);
		}
	}

	private Material RebuildMaterial()
	{
		NGUITools.DestroyImmediate(this.mDynamicMat);
		this.CreateMaterial();
		this.mDynamicMat.renderQueue = this.mRenderQueue;
		if (this.mTexture != null)
		{
			this.mDynamicMat.mainTexture = this.mTexture;
		}
		if (this.mRenderer != null)
		{
			this.mRenderer.sharedMaterials = new Material[]
			{
				this.mDynamicMat
			};
		}
		return this.mDynamicMat;
	}

	private void UpdateMaterials()
	{
		if (this.mRebuildMat || this.mDynamicMat == null || this.mClipCount != this.panel.clipCount || this.mTextureClip != (this.panel.clipping == UIDrawCall.Clipping.TextureMask))
		{
			this.RebuildMaterial();
			this.mRebuildMat = false;
		}
		else if (this.mRenderer.sharedMaterial != this.mDynamicMat)
		{
			this.mRenderer.sharedMaterials = new Material[]
			{
				this.mDynamicMat
			};
		}
	}

	public void UpdateGeometry(int widgetCount)
	{
		this.widgetCount = widgetCount;
		int size = this.verts.size;
		if (size > 0 && size == this.uvs.size && size == this.cols.size && size % 4 == 0)
		{
			if (this.mFilter == null)
			{
				this.mFilter = base.gameObject.GetComponent<MeshFilter>();
			}
			if (this.mFilter == null)
			{
				this.mFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			if (this.verts.size < 65000)
			{
				int num = (size >> 1) * 3;
				bool flag = this.mIndices == null || this.mIndices.Length != num;
				if (this.mMesh == null)
				{
					this.mMesh = new Mesh();
					this.mMesh.hideFlags = HideFlags.DontSave;
					this.mMesh.name = ((!(this.mMaterial != null)) ? "[NGUI] Mesh" : ("[NGUI] " + this.mMaterial.name));
					this.mMesh.MarkDynamic();
					flag = true;
				}
				bool flag2 = this.uvs.buffer.Length != this.verts.buffer.Length || this.cols.buffer.Length != this.verts.buffer.Length || (this.norms.buffer != null && this.norms.buffer.Length != this.verts.buffer.Length) || (this.tans.buffer != null && this.tans.buffer.Length != this.verts.buffer.Length);
				if (!flag2 && this.panel != null && this.panel.renderQueue != UIPanel.RenderQueue.Automatic)
				{
					flag2 = (this.mMesh == null || this.mMesh.vertexCount != this.verts.buffer.Length);
				}
				if (!flag2 && this.verts.size << 1 < this.verts.buffer.Length)
				{
					flag2 = true;
				}
				this.mTriangles = this.verts.size >> 1;
				if (flag2 || this.verts.buffer.Length > 65000)
				{
					if (flag2 || this.mMesh.vertexCount != this.verts.size)
					{
						this.mMesh.Clear();
						flag = true;
					}
					this.mMesh.vertices = this.verts.ToArray();
					this.mMesh.uv = this.uvs.ToArray();
					this.mMesh.colors32 = this.cols.ToArray();
					if (this.norms != null)
					{
						this.mMesh.normals = this.norms.ToArray();
					}
					if (this.tans != null)
					{
						this.mMesh.tangents = this.tans.ToArray();
					}
				}
				else
				{
					if (this.mMesh.vertexCount != this.verts.buffer.Length)
					{
						this.mMesh.Clear();
						flag = true;
					}
					this.mMesh.vertices = this.verts.buffer;
					this.mMesh.uv = this.uvs.buffer;
					this.mMesh.colors32 = this.cols.buffer;
					if (this.norms != null)
					{
						this.mMesh.normals = this.norms.buffer;
					}
					if (this.tans != null)
					{
						this.mMesh.tangents = this.tans.buffer;
					}
				}
				if (flag)
				{
					this.mIndices = this.GenerateCachedIndexBuffer(size, num);
					this.mMesh.triangles = this.mIndices;
				}
				if (flag2 || !this.alwaysOnScreen)
				{
					this.mMesh.RecalculateBounds();
				}
				this.mFilter.mesh = this.mMesh;
			}
			else
			{
				this.mTriangles = 0;
				if (this.mFilter.mesh != null)
				{
					this.mFilter.mesh.Clear();
				}
				Debug.LogError("Too many vertices on one panel: " + this.verts.size);
			}
			if (this.mRenderer == null)
			{
				this.mRenderer = base.gameObject.GetComponent<MeshRenderer>();
			}
			if (this.mRenderer == null)
			{
				this.mRenderer = base.gameObject.AddComponent<MeshRenderer>();
			}
			this.UpdateMaterials();
		}
		else
		{
			if (this.mFilter.mesh != null)
			{
				this.mFilter.mesh.Clear();
			}
			Debug.LogError("UIWidgets must fill the buffer with 4 vertices per quad. Found " + size);
		}
		this.verts.Clear();
		this.uvs.Clear();
		this.cols.Clear();
		this.norms.Clear();
		this.tans.Clear();
	}

	private int[] GenerateCachedIndexBuffer(int vertexCount, int indexCount)
	{
		int i = 0;
		int count = UIDrawCall.mCache.Count;
		while (i < count)
		{
			int[] array = UIDrawCall.mCache[i];
			if (array != null && array.Length == indexCount)
			{
				return array;
			}
			i++;
		}
		int[] array2 = new int[indexCount];
		int num = 0;
		for (int j = 0; j < vertexCount; j += 4)
		{
			array2[num++] = j;
			array2[num++] = j + 1;
			array2[num++] = j + 2;
			array2[num++] = j + 2;
			array2[num++] = j + 3;
			array2[num++] = j;
		}
		if (UIDrawCall.mCache.Count > 10)
		{
			UIDrawCall.mCache.RemoveAt(0);
		}
		UIDrawCall.mCache.Add(array2);
		return array2;
	}

	private void OnWillRenderObject()
	{
		this.UpdateMaterials();
		if (this.onRender != null)
		{
			this.onRender(this.mDynamicMat ?? this.mMaterial);
		}
		if (this.mDynamicMat == null || this.mClipCount == 0)
		{
			return;
		}
		if (this.mTextureClip)
		{
			Vector4 drawCallClipRange = this.panel.drawCallClipRange;
			Vector2 clipSoftness = this.panel.clipSoftness;
			Vector2 vector = new Vector2(1000f, 1000f);
			if (clipSoftness.x > 0f)
			{
				vector.x = drawCallClipRange.z / clipSoftness.x;
			}
			if (clipSoftness.y > 0f)
			{
				vector.y = drawCallClipRange.w / clipSoftness.y;
			}
			this.mDynamicMat.SetVector(UIDrawCall.ClipRange[0], new Vector4(-drawCallClipRange.x / drawCallClipRange.z, -drawCallClipRange.y / drawCallClipRange.w, 1f / drawCallClipRange.z, 1f / drawCallClipRange.w));
			this.mDynamicMat.SetTexture("_ClipTex", this.clipTexture);
		}
		else if (!this.mLegacyShader)
		{
			UIPanel parentPanel = this.panel;
			int num = 0;
			while (parentPanel != null)
			{
				if (parentPanel.hasClipping)
				{
					float angle = 0f;
					Vector4 drawCallClipRange2 = parentPanel.drawCallClipRange;
					if (parentPanel != this.panel)
					{
						Vector3 vector2 = parentPanel.cachedTransform.InverseTransformPoint(this.panel.cachedTransform.position);
						drawCallClipRange2.x -= vector2.x;
						drawCallClipRange2.y -= vector2.y;
						Vector3 eulerAngles = this.panel.cachedTransform.rotation.eulerAngles;
						Vector3 eulerAngles2 = parentPanel.cachedTransform.rotation.eulerAngles;
						Vector3 vector3 = eulerAngles2 - eulerAngles;
						vector3.x = NGUIMath.WrapAngle(vector3.x);
						vector3.y = NGUIMath.WrapAngle(vector3.y);
						vector3.z = NGUIMath.WrapAngle(vector3.z);
						if (Mathf.Abs(vector3.x) > 0.001f || Mathf.Abs(vector3.y) > 0.001f)
						{
							Debug.LogWarning("Panel can only be clipped properly if X and Y rotation is left at 0", this.panel);
						}
						angle = vector3.z;
					}
					this.SetClipping(num++, drawCallClipRange2, parentPanel.clipSoftness, angle);
				}
				parentPanel = parentPanel.parentPanel;
			}
		}
		else
		{
			Vector2 clipSoftness2 = this.panel.clipSoftness;
			Vector4 drawCallClipRange3 = this.panel.drawCallClipRange;
			Vector2 mainTextureOffset = new Vector2(-drawCallClipRange3.x / drawCallClipRange3.z, -drawCallClipRange3.y / drawCallClipRange3.w);
			Vector2 mainTextureScale = new Vector2(1f / drawCallClipRange3.z, 1f / drawCallClipRange3.w);
			Vector2 v = new Vector2(1000f, 1000f);
			if (clipSoftness2.x > 0f)
			{
				v.x = drawCallClipRange3.z / clipSoftness2.x;
			}
			if (clipSoftness2.y > 0f)
			{
				v.y = drawCallClipRange3.w / clipSoftness2.y;
			}
			this.mDynamicMat.mainTextureOffset = mainTextureOffset;
			this.mDynamicMat.mainTextureScale = mainTextureScale;
			this.mDynamicMat.SetVector("_ClipSharpness", v);
		}
	}

	private void SetClipping(int index, Vector4 cr, Vector2 soft, float angle)
	{
		angle *= -0.0174532924f;
		Vector2 vector = new Vector2(1000f, 1000f);
		if (soft.x > 0f)
		{
			vector.x = cr.z / soft.x;
		}
		if (soft.y > 0f)
		{
			vector.y = cr.w / soft.y;
		}
		if (index < UIDrawCall.ClipRange.Length)
		{
			this.mDynamicMat.SetVector(UIDrawCall.ClipRange[index], new Vector4(-cr.x / cr.z, -cr.y / cr.w, 1f / cr.z, 1f / cr.w));
			this.mDynamicMat.SetVector(UIDrawCall.ClipArgs[index], new Vector4(vector.x, vector.y, Mathf.Sin(angle), Mathf.Cos(angle)));
		}
	}

	private void Awake()
	{
		if (UIDrawCall.ClipRange == null)
		{
			UIDrawCall.ClipRange = new int[]
			{
				Shader.PropertyToID("_ClipRange0"),
				Shader.PropertyToID("_ClipRange1"),
				Shader.PropertyToID("_ClipRange2"),
				Shader.PropertyToID("_ClipRange4")
			};
		}
		if (UIDrawCall.ClipArgs == null)
		{
			UIDrawCall.ClipArgs = new int[]
			{
				Shader.PropertyToID("_ClipArgs0"),
				Shader.PropertyToID("_ClipArgs1"),
				Shader.PropertyToID("_ClipArgs2"),
				Shader.PropertyToID("_ClipArgs3")
			};
		}
	}

	private void OnEnable()
	{
		this.mRebuildMat = true;
	}

	private void OnDisable()
	{
		this.depthStart = 2147483647;
		this.depthEnd = -2147483648;
		this.panel = null;
		this.manager = null;
		this.mMaterial = null;
		this.mTexture = null;
		this.clipTexture = null;
		if (this.mRenderer != null)
		{
			this.mRenderer.sharedMaterials = new Material[0];
		}
		NGUITools.DestroyImmediate(this.mDynamicMat);
		this.mDynamicMat = null;
	}

	private void OnDestroy()
	{
		NGUITools.DestroyImmediate(this.mMesh);
		this.mMesh = null;
	}

	public static UIDrawCall Create(UIPanel panel, Material mat, Texture tex, Shader shader)
	{
		return UIDrawCall.Create(null, panel, mat, tex, shader);
	}

	private static UIDrawCall Create(string name, UIPanel pan, Material mat, Texture tex, Shader shader)
	{
		UIDrawCall uIDrawCall = UIDrawCall.Create(name);
		uIDrawCall.gameObject.layer = pan.cachedGameObject.layer;
		uIDrawCall.baseMaterial = mat;
		uIDrawCall.mainTexture = tex;
		uIDrawCall.shader = shader;
		uIDrawCall.renderQueue = pan.startingRenderQueue;
		uIDrawCall.sortingOrder = pan.sortingOrder;
		uIDrawCall.manager = pan;
		return uIDrawCall;
	}

	private static UIDrawCall Create(string name)
	{
		if (UIDrawCall.mInactiveList.size > 0)
		{
			UIDrawCall uIDrawCall = UIDrawCall.mInactiveList.Pop();
			UIDrawCall.mActiveList.Add(uIDrawCall);
			if (name != null)
			{
				uIDrawCall.name = name;
			}
			NGUITools.SetActive(uIDrawCall.gameObject, true);
			return uIDrawCall;
		}
		GameObject gameObject = new GameObject(name);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		UIDrawCall uIDrawCall2 = gameObject.AddComponent<UIDrawCall>();
		UIDrawCall.mActiveList.Add(uIDrawCall2);
		return uIDrawCall2;
	}

	public static void ClearAll()
	{
		bool isPlaying = Application.isPlaying;
		int i = UIDrawCall.mActiveList.size;
		while (i > 0)
		{
			UIDrawCall uIDrawCall = UIDrawCall.mActiveList[--i];
			if (uIDrawCall)
			{
				if (isPlaying)
				{
					NGUITools.SetActive(uIDrawCall.gameObject, false);
				}
				else
				{
					NGUITools.DestroyImmediate(uIDrawCall.gameObject);
				}
			}
		}
		UIDrawCall.mActiveList.Clear();
	}

	public static void ReleaseAll()
	{
		UIDrawCall.ClearAll();
		UIDrawCall.ReleaseInactive();
	}

	public static void ReleaseInactive()
	{
		int i = UIDrawCall.mInactiveList.size;
		while (i > 0)
		{
			UIDrawCall uIDrawCall = UIDrawCall.mInactiveList[--i];
			if (uIDrawCall)
			{
				NGUITools.DestroyImmediate(uIDrawCall.gameObject);
			}
		}
		UIDrawCall.mInactiveList.Clear();
	}

	public static int Count(UIPanel panel)
	{
		int num = 0;
		for (int i = 0; i < UIDrawCall.mActiveList.size; i++)
		{
			if (UIDrawCall.mActiveList[i].manager == panel)
			{
				num++;
			}
		}
		return num;
	}

	public static void Destroy(UIDrawCall dc)
	{
		if (dc)
		{
			dc.onRender = null;
			if (Application.isPlaying)
			{
				if (UIDrawCall.mActiveList.Remove(dc))
				{
					NGUITools.SetActive(dc.gameObject, false);
					UIDrawCall.mInactiveList.Add(dc);
				}
			}
			else
			{
				UIDrawCall.mActiveList.Remove(dc);
				NGUITools.DestroyImmediate(dc.gameObject);
			}
		}
	}
}
