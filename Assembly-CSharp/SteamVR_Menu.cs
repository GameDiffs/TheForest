using System;
using UnityEngine;
using Valve.VR;

public class SteamVR_Menu : MonoBehaviour
{
	public Texture cursor;

	public Texture background;

	public Texture logo;

	public float logoHeight;

	public float menuOffset;

	public Vector2 scaleLimits = new Vector2(0.1f, 5f);

	public float scaleRate = 0.5f;

	private SteamVR_Overlay overlay;

	private Camera overlayCam;

	private Vector4 uvOffset;

	private float distance;

	private string scaleLimitX;

	private string scaleLimitY;

	private string scaleRateText;

	private CursorLockMode savedCursorLockState;

	private bool savedCursorVisible;

	public RenderTexture texture
	{
		get
		{
			return (!this.overlay) ? null : (this.overlay.texture as RenderTexture);
		}
	}

	public float scale
	{
		get;
		private set;
	}

	private void Awake()
	{
		this.scaleLimitX = string.Format("{0:N1}", this.scaleLimits.x);
		this.scaleLimitY = string.Format("{0:N1}", this.scaleLimits.y);
		this.scaleRateText = string.Format("{0:N1}", this.scaleRate);
		SteamVR_Overlay instance = SteamVR_Overlay.instance;
		if (instance != null)
		{
			this.uvOffset = instance.uvOffset;
			this.distance = instance.distance;
		}
	}

	private void OnGUI()
	{
		if (this.overlay == null)
		{
			return;
		}
		RenderTexture renderTexture = this.overlay.texture as RenderTexture;
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = renderTexture;
		if (Event.current.type == EventType.Repaint)
		{
			GL.Clear(false, true, Color.clear);
		}
		Rect screenRect = new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height);
		if (Screen.width < renderTexture.width)
		{
			screenRect.width = (float)Screen.width;
			this.overlay.uvOffset.x = -(float)(renderTexture.width - Screen.width) / (float)(2 * renderTexture.width);
		}
		if (Screen.height < renderTexture.height)
		{
			screenRect.height = (float)Screen.height;
			this.overlay.uvOffset.y = (float)(renderTexture.height - Screen.height) / (float)(2 * renderTexture.height);
		}
		GUILayout.BeginArea(screenRect);
		if (this.background != null)
		{
			GUI.DrawTexture(new Rect((screenRect.width - (float)this.background.width) / 2f, (screenRect.height - (float)this.background.height) / 2f, (float)this.background.width, (float)this.background.height), this.background);
		}
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		if (this.logo != null)
		{
			GUILayout.Space(screenRect.height / 2f - this.logoHeight);
			GUILayout.Box(this.logo, new GUILayoutOption[0]);
		}
		GUILayout.Space(this.menuOffset);
		bool flag = GUILayout.Button("[Esc] - Close menu", new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label(string.Format("Scale: {0:N4}", this.scale), new GUILayoutOption[0]);
		float num = GUILayout.HorizontalSlider(this.scale, this.scaleLimits.x, this.scaleLimits.y, new GUILayoutOption[0]);
		if (num != this.scale)
		{
			this.SetScale(num);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label(string.Format("Scale limits:", new object[0]), new GUILayoutOption[0]);
		string text = GUILayout.TextField(this.scaleLimitX, new GUILayoutOption[0]);
		if (text != this.scaleLimitX && float.TryParse(text, out this.scaleLimits.x))
		{
			this.scaleLimitX = text;
		}
		string text2 = GUILayout.TextField(this.scaleLimitY, new GUILayoutOption[0]);
		if (text2 != this.scaleLimitY && float.TryParse(text2, out this.scaleLimits.y))
		{
			this.scaleLimitY = text2;
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label(string.Format("Scale rate:", new object[0]), new GUILayoutOption[0]);
		string text3 = GUILayout.TextField(this.scaleRateText, new GUILayoutOption[0]);
		if (text3 != this.scaleRateText && float.TryParse(text3, out this.scaleRate))
		{
			this.scaleRateText = text3;
		}
		GUILayout.EndHorizontal();
		if (SteamVR.active)
		{
			SteamVR instance = SteamVR.instance;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			float sceneResolutionScale = SteamVR_Camera.sceneResolutionScale;
			int num2 = (int)(instance.sceneWidth * sceneResolutionScale);
			int num3 = (int)(instance.sceneHeight * sceneResolutionScale);
			int num4 = (int)(100f * sceneResolutionScale);
			GUILayout.Label(string.Format("Scene quality: {0}x{1} ({2}%)", num2, num3, num4), new GUILayoutOption[0]);
			int num5 = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)num4, 50f, 200f, new GUILayoutOption[0]));
			if (num5 != num4)
			{
				SteamVR_Camera.sceneResolutionScale = (float)num5 / 100f;
			}
			GUILayout.EndHorizontal();
		}
		this.overlay.highquality = GUILayout.Toggle(this.overlay.highquality, "High quality", new GUILayoutOption[0]);
		if (this.overlay.highquality)
		{
			this.overlay.curved = GUILayout.Toggle(this.overlay.curved, "Curved overlay", new GUILayoutOption[0]);
			this.overlay.antialias = GUILayout.Toggle(this.overlay.antialias, "Overlay RGSS(2x2)", new GUILayoutOption[0]);
		}
		else
		{
			this.overlay.curved = false;
			this.overlay.antialias = false;
		}
		SteamVR_Camera steamVR_Camera = SteamVR_Render.Top();
		if (steamVR_Camera != null)
		{
			steamVR_Camera.wireframe = GUILayout.Toggle(steamVR_Camera.wireframe, "Wireframe", new GUILayoutOption[0]);
			SteamVR_Render instance2 = SteamVR_Render.instance;
			if (instance2.trackingSpace == ETrackingUniverseOrigin.TrackingUniverseSeated)
			{
				if (GUILayout.Button("Switch to Standing", new GUILayoutOption[0]))
				{
					instance2.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseStanding;
				}
				if (GUILayout.Button("Center View", new GUILayoutOption[0]))
				{
					CVRSystem system = OpenVR.System;
					if (system != null)
					{
						system.ResetSeatedZeroPose();
					}
				}
			}
			else if (GUILayout.Button("Switch to Seated", new GUILayoutOption[0]))
			{
				instance2.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseSeated;
			}
		}
		if (GUILayout.Button("Exit", new GUILayoutOption[0]))
		{
			Application.Quit();
		}
		GUILayout.Space(this.menuOffset);
		string environmentVariable = Environment.GetEnvironmentVariable("VR_OVERRIDE");
		if (environmentVariable != null)
		{
			GUILayout.Label("VR_OVERRIDE=" + environmentVariable, new GUILayoutOption[0]);
		}
		GUILayout.Label("Graphics device: " + SystemInfo.graphicsDeviceVersion, new GUILayoutOption[0]);
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		if (this.cursor != null)
		{
			float x = Input.mousePosition.x;
			float top = (float)Screen.height - Input.mousePosition.y;
			float width = (float)this.cursor.width;
			float height = (float)this.cursor.height;
			GUI.DrawTexture(new Rect(x, top, width, height), this.cursor);
		}
		RenderTexture.active = active;
		if (flag)
		{
			this.HideMenu();
		}
	}

	public void ShowMenu()
	{
		SteamVR_Overlay instance = SteamVR_Overlay.instance;
		if (instance == null)
		{
			return;
		}
		RenderTexture renderTexture = instance.texture as RenderTexture;
		if (renderTexture == null)
		{
			Debug.LogError("Menu requires overlay texture to be a render texture.");
			return;
		}
		this.SaveCursorState();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		this.overlay = instance;
		this.uvOffset = instance.uvOffset;
		this.distance = instance.distance;
		Camera[] array = UnityEngine.Object.FindObjectsOfType(typeof(Camera)) as Camera[];
		Camera[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Camera camera = array2[i];
			if (camera.enabled && camera.targetTexture == renderTexture)
			{
				this.overlayCam = camera;
				this.overlayCam.enabled = false;
				break;
			}
		}
		SteamVR_Camera steamVR_Camera = SteamVR_Render.Top();
		if (steamVR_Camera != null)
		{
			this.scale = steamVR_Camera.origin.localScale.x;
		}
	}

	public void HideMenu()
	{
		this.RestoreCursorState();
		if (this.overlayCam != null)
		{
			this.overlayCam.enabled = true;
		}
		if (this.overlay != null)
		{
			this.overlay.uvOffset = this.uvOffset;
			this.overlay.distance = this.distance;
			this.overlay = null;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
		{
			if (this.overlay == null)
			{
				this.ShowMenu();
			}
			else
			{
				this.HideMenu();
			}
		}
		else if (Input.GetKeyDown(KeyCode.Home))
		{
			this.SetScale(1f);
		}
		else if (Input.GetKey(KeyCode.PageUp))
		{
			this.SetScale(Mathf.Clamp(this.scale + this.scaleRate * Time.deltaTime, this.scaleLimits.x, this.scaleLimits.y));
		}
		else if (Input.GetKey(KeyCode.PageDown))
		{
			this.SetScale(Mathf.Clamp(this.scale - this.scaleRate * Time.deltaTime, this.scaleLimits.x, this.scaleLimits.y));
		}
	}

	private void SetScale(float scale)
	{
		this.scale = scale;
		SteamVR_Camera steamVR_Camera = SteamVR_Render.Top();
		if (steamVR_Camera != null)
		{
			steamVR_Camera.origin.localScale = new Vector3(scale, scale, scale);
		}
	}

	private void SaveCursorState()
	{
		this.savedCursorVisible = Cursor.visible;
		this.savedCursorLockState = Cursor.lockState;
	}

	private void RestoreCursorState()
	{
		Cursor.visible = this.savedCursorVisible;
		Cursor.lockState = this.savedCursorLockState;
	}
}
