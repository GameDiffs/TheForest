using OrbCreationExtensions;
using System;
using UnityEngine;

public class LODSwitcher : MonoBehaviour
{
	public Mesh[] lodMeshes;

	public GameObject[] lodGameObjects;

	public float[] lodScreenSizes;

	public float deactivateAtDistance;

	public Camera customCamera;

	private MeshFilter meshFilter;

	private SkinnedMeshRenderer skinnedMeshRenderer;

	private Vector3 centerOffset;

	private float pixelsPerMeter;

	private float objectSize;

	private int fixedLODLevel = -1;

	private int lodLevel;

	private int frameOffset;

	private int frameInterval = 10;

	private void Start()
	{
		this.frameOffset = Mathf.RoundToInt(UnityEngine.Random.value * 10f);
		if ((this.lodMeshes == null || this.lodMeshes.Length <= 0) && (this.lodGameObjects == null || this.lodGameObjects.Length <= 0))
		{
			Debug.LogWarning(base.gameObject.name + ".LODSwitcher: No lodMeshes/lodGameObjects set. LODSwitcher is now disabled.");
			base.enabled = false;
		}
		int num = 0;
		if (this.lodMeshes != null)
		{
			num = this.lodMeshes.Length - 1;
		}
		if (this.lodGameObjects != null)
		{
			num = Mathf.Max(num, this.lodGameObjects.Length - 1);
		}
		if (base.enabled && (this.lodScreenSizes == null || this.lodScreenSizes.Length != num))
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				base.gameObject.name,
				".LODSwitcher: lodScreenSizes should have a length of ",
				num,
				". LODSwitcher is now disabled."
			}));
			base.enabled = false;
		}
		this.SetLODLevel(0);
		this.ComputeDimensions();
		this.lodLevel = -1;
		this.ComputeLODLevel();
	}

	public void ComputeDimensions()
	{
		Bounds bounds = base.gameObject.GetWorldBounds();
		this.centerOffset = bounds.center - base.transform.position;
		this.objectSize = bounds.size.magnitude;
		if (this.skinnedMeshRenderer == null && this.meshFilter == null)
		{
			this.GetMeshFilter();
		}
		if (this.skinnedMeshRenderer != null)
		{
			bounds = this.skinnedMeshRenderer.localBounds;
			this.objectSize = bounds.size.magnitude;
			this.centerOffset = bounds.center;
			this.frameInterval = 1;
		}
		Camera main = this.customCamera;
		if (main == null)
		{
			main = Camera.main;
		}
		if (main == null)
		{
			Debug.LogWarning("No scene camera found yet, you need to call LODSwitcher.ComputeDimensions() again when you have your Camera set up");
			return;
		}
		Vector3 a = main.ScreenToWorldPoint(new Vector3(((float)Screen.width - 100f) / 2f, 0f, 1f));
		Vector3 b = main.ScreenToWorldPoint(new Vector3(((float)Screen.width + 100f) / 2f, 0f, 1f));
		this.pixelsPerMeter = 1f / (Vector3.Distance(a, b) / 100f);
	}

	public void SetCustomCamera(Camera aCamera)
	{
		this.customCamera = aCamera;
		this.ComputeDimensions();
	}

	public void SetFixedLODLevel(int aLevel)
	{
		this.fixedLODLevel = Mathf.Max(0, Mathf.Min(this.MaxLODLevel(), aLevel));
	}

	public void ReleaseFixedLODLevel()
	{
		this.fixedLODLevel = -1;
	}

	public int GetLODLevel()
	{
		return this.lodLevel;
	}

	public int MaxLODLevel()
	{
		if (this.lodScreenSizes == null)
		{
			return 0;
		}
		return this.lodScreenSizes.Length;
	}

	public Mesh GetMesh(int aLevel)
	{
		if (this.lodMeshes != null && this.lodMeshes.Length >= aLevel)
		{
			return this.lodMeshes[aLevel];
		}
		return null;
	}

	public void SetMesh(Mesh aMesh, int aLevel)
	{
		if (this.lodMeshes == null)
		{
			this.lodMeshes = new Mesh[aLevel + 1];
		}
		if (this.lodMeshes.Length <= aLevel)
		{
			Array.Resize<Mesh>(ref this.lodMeshes, aLevel + 1);
		}
		if (aLevel > 0)
		{
			if (this.lodScreenSizes == null)
			{
				this.lodScreenSizes = new float[aLevel];
			}
			if (this.lodScreenSizes.Length < aLevel)
			{
				Array.Resize<float>(ref this.lodScreenSizes, aLevel);
			}
		}
		this.lodMeshes[aLevel] = aMesh;
		if (aLevel == this.lodLevel)
		{
			this.lodLevel = -1;
			this.SetLODLevel(aLevel);
		}
		this.ComputeDimensions();
	}

	public void SetRelativeScreenSize(float aValue, int aLevel)
	{
		if (this.lodScreenSizes == null)
		{
			this.lodScreenSizes = new float[aLevel];
		}
		if (this.lodScreenSizes.Length < aLevel)
		{
			Array.Resize<float>(ref this.lodScreenSizes, aLevel);
		}
		for (int i = 0; i < this.lodScreenSizes.Length; i++)
		{
			if (i + 1 == aLevel)
			{
				this.lodScreenSizes[i] = aValue;
			}
			else if (this.lodScreenSizes[i] == 0f)
			{
				if (i == 0)
				{
					this.lodScreenSizes[i] = 0.6f;
				}
				else
				{
					this.lodScreenSizes[i] = this.lodScreenSizes[i - 1] * 0.5f;
				}
			}
		}
	}

	private void Update()
	{
		if ((Time.frameCount + this.frameOffset) % this.frameInterval != 0)
		{
			return;
		}
		this.ComputeLODLevel();
	}

	public Vector3 NearestCameraPositionForLOD(int aLevel)
	{
		this.ComputeDimensions();
		Camera main = this.customCamera;
		if (main == null)
		{
			main = Camera.main;
		}
		if (aLevel > 0 && aLevel <= this.lodScreenSizes.Length)
		{
			float num = this.objectSize * this.pixelsPerMeter;
			float d = num / (float)Screen.width / this.lodScreenSizes[aLevel - 1];
			return base.transform.position + this.centerOffset + main.transform.rotation * (Vector3.back * d);
		}
		return main.transform.position;
	}

	public float ScreenPortion()
	{
		Camera main = this.customCamera;
		if (main == null)
		{
			main = Camera.main;
		}
		float num = Vector3.Distance(main.transform.position, base.transform.position + this.centerOffset);
		if (this.deactivateAtDistance > 0f && num > this.deactivateAtDistance)
		{
			return -1f;
		}
		float num2 = this.objectSize * this.pixelsPerMeter;
		float num3 = num2 / num / (float)Screen.width;
		return (float)Mathf.RoundToInt(num3 * 40f) * 0.025f;
	}

	private void ComputeLODLevel()
	{
		int num = 0;
		if (this.fixedLODLevel >= 0)
		{
			num = this.fixedLODLevel;
		}
		else
		{
			float num2 = this.ScreenPortion();
			if (num2 >= 0f)
			{
				for (int i = 0; i < this.lodScreenSizes.Length; i++)
				{
					if (num2 < this.lodScreenSizes[i])
					{
						num++;
					}
				}
			}
			else
			{
				num = -1;
			}
		}
		if (num != this.lodLevel)
		{
			this.SetLODLevel(num);
		}
	}

	private void GetMeshFilter()
	{
		this.skinnedMeshRenderer = base.gameObject.GetComponent<SkinnedMeshRenderer>();
		if (this.skinnedMeshRenderer == null)
		{
			this.meshFilter = base.gameObject.GetComponent<MeshFilter>();
		}
	}

	public void SetLODLevel(int newLodLevel)
	{
		if (newLodLevel != this.lodLevel)
		{
			newLodLevel = Mathf.Min(this.MaxLODLevel(), newLodLevel);
			if (newLodLevel < 0)
			{
				base.gameObject.GetComponent<Renderer>().enabled = false;
			}
			else
			{
				if (this.lodMeshes != null && this.lodMeshes.Length > 0)
				{
					base.gameObject.GetComponent<Renderer>().enabled = true;
				}
				if (this.lodMeshes != null && this.lodMeshes.Length > newLodLevel)
				{
					if (this.skinnedMeshRenderer == null && this.meshFilter == null)
					{
						this.GetMeshFilter();
					}
					if (this.skinnedMeshRenderer != null)
					{
						this.skinnedMeshRenderer.sharedMesh = this.lodMeshes[newLodLevel];
					}
					else if (this.meshFilter != null)
					{
						this.meshFilter.sharedMesh = this.lodMeshes[newLodLevel];
					}
				}
				int num = 0;
				while (this.lodGameObjects != null && num < this.lodGameObjects.Length)
				{
					this.lodGameObjects[num].SetActive(num == newLodLevel);
					num++;
				}
			}
			this.lodLevel = newLodLevel;
		}
	}
}
