using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using Valve.VR;

public class SteamVR_ExternalCamera : MonoBehaviour
{
	public struct Config
	{
		public float x;

		public float y;

		public float z;

		public float rx;

		public float ry;

		public float rz;

		public float fov;

		public float near;

		public float far;

		public float sceneResolutionScale;

		public float frameSkip;

		public float nearOffset;

		public float farOffset;

		public float hmdOffset;

		public bool disableStandardAssets;
	}

	public SteamVR_ExternalCamera.Config config;

	public string configPath;

	private Camera cam;

	private Transform target;

	private GameObject clipQuad;

	private Material clipMaterial;

	private Material colorMat;

	private Material alphaMat;

	private Camera[] cameras;

	private Rect[] cameraRects;

	private float sceneResolutionScale;

	public void ReadConfig()
	{
		try
		{
			HmdMatrix34_t pose = default(HmdMatrix34_t);
			bool flag = false;
			object obj = this.config;
			string[] array = File.ReadAllLines(this.configPath);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				string[] array3 = text.Split(new char[]
				{
					'='
				});
				if (array3.Length == 2)
				{
					string text2 = array3[0];
					if (text2 == "m")
					{
						string[] array4 = array3[1].Split(new char[]
						{
							','
						});
						if (array4.Length == 12)
						{
							pose.m0 = float.Parse(array4[0]);
							pose.m1 = float.Parse(array4[1]);
							pose.m2 = float.Parse(array4[2]);
							pose.m3 = float.Parse(array4[3]);
							pose.m4 = float.Parse(array4[4]);
							pose.m5 = float.Parse(array4[5]);
							pose.m6 = float.Parse(array4[6]);
							pose.m7 = float.Parse(array4[7]);
							pose.m8 = float.Parse(array4[8]);
							pose.m9 = float.Parse(array4[9]);
							pose.m10 = float.Parse(array4[10]);
							pose.m11 = float.Parse(array4[11]);
							flag = true;
						}
					}
					else if (text2 == "disableStandardAssets")
					{
						FieldInfo field = obj.GetType().GetField(text2);
						if (field != null)
						{
							field.SetValue(obj, bool.Parse(array3[1]));
						}
					}
					else
					{
						FieldInfo field2 = obj.GetType().GetField(text2);
						if (field2 != null)
						{
							field2.SetValue(obj, float.Parse(array3[1]));
						}
					}
				}
			}
			this.config = (SteamVR_ExternalCamera.Config)obj;
			if (flag)
			{
				SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(pose);
				this.config.x = rigidTransform.pos.x;
				this.config.y = rigidTransform.pos.y;
				this.config.z = rigidTransform.pos.z;
				Vector3 eulerAngles = rigidTransform.rot.eulerAngles;
				this.config.rx = eulerAngles.x;
				this.config.ry = eulerAngles.y;
				this.config.rz = eulerAngles.z;
			}
		}
		catch
		{
		}
	}

	public void AttachToCamera(SteamVR_Camera vrcam)
	{
		if (this.target == vrcam.head)
		{
			return;
		}
		this.target = vrcam.head;
		Transform parent = base.transform.parent;
		Transform parent2 = vrcam.head.parent;
		parent.parent = parent2;
		parent.localPosition = Vector3.zero;
		parent.localRotation = Quaternion.identity;
		parent.localScale = Vector3.one;
		vrcam.enabled = false;
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(vrcam.gameObject);
		vrcam.enabled = true;
		gameObject.name = "camera";
		UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<SteamVR_Camera>());
		UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<SteamVR_CameraFlip>());
		this.cam = gameObject.GetComponent<Camera>();
		this.cam.fieldOfView = this.config.fov;
		this.cam.useOcclusionCulling = false;
		this.cam.enabled = false;
		this.colorMat = new Material(Shader.Find("Custom/SteamVR_ColorOut"));
		this.alphaMat = new Material(Shader.Find("Custom/SteamVR_AlphaOut"));
		this.clipMaterial = new Material(Shader.Find("Custom/SteamVR_ClearAll"));
		Transform transform = gameObject.transform;
		transform.parent = base.transform;
		transform.localPosition = new Vector3(this.config.x, this.config.y, this.config.z);
		transform.localRotation = Quaternion.Euler(this.config.rx, this.config.ry, this.config.rz);
		transform.localScale = Vector3.one;
		while (transform.childCount > 0)
		{
			UnityEngine.Object.DestroyImmediate(transform.GetChild(0).gameObject);
		}
		this.clipQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
		this.clipQuad.name = "ClipQuad";
		UnityEngine.Object.DestroyImmediate(this.clipQuad.GetComponent<MeshCollider>());
		MeshRenderer component = this.clipQuad.GetComponent<MeshRenderer>();
		component.material = this.clipMaterial;
		component.shadowCastingMode = ShadowCastingMode.Off;
		component.receiveShadows = false;
		component.useLightProbes = false;
		component.reflectionProbeUsage = ReflectionProbeUsage.Off;
		Transform transform2 = this.clipQuad.transform;
		transform2.parent = transform;
		transform2.localScale = new Vector3(1000f, 1000f, 1f);
		transform2.localRotation = Quaternion.identity;
		this.clipQuad.SetActive(false);
	}

	public float GetTargetDistance()
	{
		if (this.target == null)
		{
			return this.config.near + 0.01f;
		}
		Transform transform = this.cam.transform;
		Vector3 vector = new Vector3(transform.forward.x, 0f, transform.forward.z);
		Vector3 normalized = vector.normalized;
		Vector3 arg_B7_0 = this.target.position;
		Vector3 vector2 = new Vector3(this.target.forward.x, 0f, this.target.forward.z);
		Vector3 inPoint = arg_B7_0 + vector2.normalized * this.config.hmdOffset;
		Plane plane = new Plane(normalized, inPoint);
		float value = -plane.GetDistanceToPoint(transform.position);
		return Mathf.Clamp(value, this.config.near + 0.01f, this.config.far - 0.01f);
	}

	public void RenderNear()
	{
		int num = Screen.width / 2;
		int num2 = Screen.height / 2;
		if (this.cam.targetTexture == null || this.cam.targetTexture.width != num || this.cam.targetTexture.height != num2)
		{
			this.cam.targetTexture = new RenderTexture(num, num2, 16, RenderTextureFormat.ARGB32);
		}
		this.cam.nearClipPlane = this.config.near;
		this.cam.farClipPlane = this.config.far;
		CameraClearFlags clearFlags = this.cam.clearFlags;
		Color backgroundColor = this.cam.backgroundColor;
		this.cam.clearFlags = CameraClearFlags.Color;
		this.cam.backgroundColor = Color.clear;
		float d = Mathf.Clamp(this.GetTargetDistance() + this.config.nearOffset, this.config.near, this.config.far);
		Transform parent = this.clipQuad.transform.parent;
		this.clipQuad.transform.position = parent.position + parent.forward * d;
		MonoBehaviour[] array = null;
		bool[] array2 = null;
		if (this.config.disableStandardAssets)
		{
			array = this.cam.gameObject.GetComponents<MonoBehaviour>();
			array2 = new bool[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				MonoBehaviour monoBehaviour = array[i];
				if (monoBehaviour.enabled && monoBehaviour.GetType().ToString().StartsWith("UnityStandardAssets."))
				{
					monoBehaviour.enabled = false;
					array2[i] = true;
				}
			}
		}
		this.clipQuad.SetActive(true);
		this.cam.Render();
		this.clipQuad.SetActive(false);
		if (array != null)
		{
			for (int j = 0; j < array.Length; j++)
			{
				if (array2[j])
				{
					array[j].enabled = true;
				}
			}
		}
		this.cam.clearFlags = clearFlags;
		this.cam.backgroundColor = backgroundColor;
		Graphics.DrawTexture(new Rect(0f, 0f, (float)num, (float)num2), this.cam.targetTexture, this.colorMat);
		Graphics.DrawTexture(new Rect((float)num, 0f, (float)num, (float)num2), this.cam.targetTexture, this.alphaMat);
	}

	public void RenderFar()
	{
		this.cam.nearClipPlane = this.config.near;
		this.cam.farClipPlane = this.config.far;
		this.cam.Render();
		int num = Screen.width / 2;
		int num2 = Screen.height / 2;
		Graphics.DrawTexture(new Rect(0f, (float)num2, (float)num, (float)num2), this.cam.targetTexture, this.colorMat);
	}

	private void OnEnable()
	{
		this.cameras = UnityEngine.Object.FindObjectsOfType<Camera>();
		if (this.cameras != null)
		{
			int num = this.cameras.Length;
			this.cameraRects = new Rect[num];
			for (int i = 0; i < num; i++)
			{
				Camera camera = this.cameras[i];
				this.cameraRects[i] = camera.rect;
				if (!(camera == this.cam))
				{
					if (!(camera.targetTexture != null))
					{
						if (!(camera.GetComponent<SteamVR_Camera>() != null))
						{
							camera.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
						}
					}
				}
			}
		}
		if (this.config.sceneResolutionScale > 0f)
		{
			this.sceneResolutionScale = SteamVR_Camera.sceneResolutionScale;
			SteamVR_Camera.sceneResolutionScale = this.config.sceneResolutionScale;
		}
	}

	private void OnDisable()
	{
		if (this.cameras != null)
		{
			int num = this.cameras.Length;
			for (int i = 0; i < num; i++)
			{
				Camera camera = this.cameras[i];
				if (camera != null)
				{
					camera.rect = this.cameraRects[i];
				}
			}
			this.cameras = null;
			this.cameraRects = null;
		}
		if (this.config.sceneResolutionScale > 0f)
		{
			SteamVR_Camera.sceneResolutionScale = this.sceneResolutionScale;
		}
	}
}
