using OrbCreationExtensions;
using System;
using UnityEngine;

public class SimpleLODDemoCamera : MonoBehaviour
{
	public float moveSpeed = 0.15f;

	public float scrollSpeed = 2f;

	public float sensitivityX = 4f;

	public float sensitivityY = 4f;

	public float minimumY = -40f;

	public float maximumY = 40f;

	public float manualRotationAcceleration = 40f;

	private Vector3 startPosition;

	private Vector3 targetPosition;

	private GameObject currentScene;

	private GameObject clickedObject;

	private int totTriangles;

	private int totSubMeshes;

	private int frameCount;

	private float frameTotTime = 60f;

	private float fps = 60f;

	private float displayFPS = 60f;

	private int lodLevel = -1;

	private float rotationX;

	private float rotationY;

	private void Start()
	{
		this.startPosition = base.transform.position;
		this.targetPosition = this.startPosition;
		base.InvokeRepeating("GetStats", 0.1f, 1f);
	}

	public void SetCurrentScene(GameObject aGO)
	{
		string text = null;
		if (this.clickedObject != null && this.clickedObject != this.currentScene)
		{
			text = this.clickedObject.name;
		}
		this.currentScene = aGO;
		if (text != null)
		{
			this.clickedObject = this.currentScene.FindFirstChildWithName(text);
		}
		else
		{
			this.clickedObject = aGO;
		}
	}

	public void SetClickedObject(GameObject aGO)
	{
		if (aGO == null)
		{
			aGO = this.currentScene;
			this.targetPosition = this.startPosition;
		}
		else
		{
			this.clickedObject = aGO;
			Bounds worldBounds = this.clickedObject.GetWorldBounds();
			this.targetPosition = worldBounds.center + base.transform.rotation * (Vector3.back * (worldBounds.extents.magnitude + 0.5f));
		}
		this.GetStats();
	}

	private void GetStats()
	{
		Mesh[] meshes = this.clickedObject.GetMeshes(false);
		this.totSubMeshes = 0;
		this.totTriangles = 0;
		if (meshes != null)
		{
			Mesh[] array = meshes;
			for (int i = 0; i < array.Length; i++)
			{
				Mesh mesh = array[i];
				if (mesh != null)
				{
					this.totSubMeshes += mesh.subMeshCount;
					this.totTriangles += mesh.GetTriangleCount();
				}
			}
		}
		if (this.clickedObject != this.currentScene)
		{
			LODSwitcher firstComponentInChildren = this.clickedObject.GetFirstComponentInChildren<LODSwitcher>();
			if (firstComponentInChildren)
			{
				this.lodLevel = firstComponentInChildren.GetLODLevel();
			}
			else
			{
				this.lodLevel = -1;
			}
		}
		else
		{
			this.lodLevel = -1;
		}
		if (this.frameCount > 0)
		{
			this.displayFPS = Mathf.Lerp(this.displayFPS, this.frameTotTime / (float)this.frameCount, 0.5f);
		}
		this.frameTotTime = 0f;
		this.frameCount = 0;
	}

	private void Update()
	{
		this.frameTotTime += Time.timeScale / Time.deltaTime;
		this.frameCount++;
		this.fps = this.fps * 0.9f + 1f / Time.deltaTime * 0.1f;
		base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, 3f * Time.deltaTime);
		float num = Input.GetAxis("Vertical") * this.moveSpeed;
		float num2 = Input.GetAxis("Horizontal") * this.moveSpeed * 0.8f;
		if (Input.GetMouseButton(0) || num != 0f || num2 != 0f)
		{
			this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
			this.rotationY = Mathf.Clamp(this.rotationY + Input.GetAxis("Mouse Y") * this.sensitivityY, this.minimumY, this.maximumY);
		}
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(-this.rotationY, this.rotationX, 0f), this.manualRotationAcceleration * Time.deltaTime);
		num += Input.GetAxis("Mouse ScrollWheel") * this.scrollSpeed;
		this.targetPosition += base.transform.rotation * (Vector3.forward * num);
		this.targetPosition += base.transform.rotation * (Vector3.right * num2);
		if (this.targetPosition.y < -1f)
		{
			this.targetPosition.y = -1f;
		}
	}

	private void OnGUI()
	{
		GUI.skin.label.normal.textColor = Color.black;
		string text = ((!(this.clickedObject == this.currentScene)) ? this.clickedObject.name : "All objects") + ":\n";
		text = text + this.totSubMeshes + " submeshes / materials\n";
		text = string.Concat(new object[]
		{
			text,
			this.totTriangles,
			" triangles ",
			(this.lodLevel < 0) ? string.Empty : ("(LOD " + this.lodLevel + ")")
		});
		GUI.Label(new Rect(2f, 2f, 200f, 100f), text);
		if (GUI.Button(new Rect((float)Screen.width - 97f, (float)(Screen.height - 27), 95f, 25f), "Reset camera"))
		{
			this.targetPosition = this.startPosition;
			this.clickedObject = this.currentScene;
			this.rotationX = 0f;
			this.rotationY = 0f;
		}
	}
}
