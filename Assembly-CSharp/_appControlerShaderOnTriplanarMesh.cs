using System;
using UnityEngine;

public class _appControlerShaderOnTriplanarMesh : MonoBehaviour
{
	public bool shadows;

	public bool forward_path = true;

	private bool panel_enabled;

	public float light_dir;

	public float model_dir;

	private void Awake()
	{
		this.panel_enabled = true;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			this.panel_enabled = !this.panel_enabled;
		}
		if (Input.GetKey(KeyCode.Period))
		{
			MouseOrbitCS mouseOrbitCS = base.GetComponent(typeof(MouseOrbitCS)) as MouseOrbitCS;
			mouseOrbitCS.distance += 1f;
			if (mouseOrbitCS.distance > 150f)
			{
				mouseOrbitCS.distance = 150f;
			}
		}
		if (Input.GetKey(KeyCode.Comma))
		{
			MouseOrbitCS mouseOrbitCS2 = base.GetComponent(typeof(MouseOrbitCS)) as MouseOrbitCS;
			mouseOrbitCS2.distance -= 1f;
			if (mouseOrbitCS2.distance < 30f)
			{
				mouseOrbitCS2.distance = 30f;
			}
		}
	}

	private void OnGUI()
	{
		GUILayout.Space(10f);
		GUILayout.BeginVertical("box", new GUILayoutOption[0]);
		GUILayout.Label(string.Empty + FPSmeter.fps, new GUILayoutOption[0]);
		if (this.panel_enabled)
		{
			this.shadows = GUILayout.Toggle(this.shadows, "disable Unity's shadows", new GUILayoutOption[0]);
			Light component = GameObject.Find("Directional light").GetComponent<Light>();
			component.shadows = ((!this.shadows) ? LightShadows.Soft : LightShadows.None);
			this.forward_path = GUILayout.Toggle(this.forward_path, "forward rendering", new GUILayoutOption[0]);
			Camera component2 = GameObject.Find("Main Camera").GetComponent<Camera>();
			component2.renderingPath = ((!this.forward_path) ? RenderingPath.DeferredLighting : RenderingPath.Forward);
			GUILayout.Label("Light", new GUILayoutOption[]
			{
				GUILayout.MaxWidth(40f)
			});
			this.light_dir = GUILayout.HorizontalSlider(this.light_dir, 0f, 360f, new GUILayoutOption[0]);
			component.transform.rotation = Quaternion.Euler(this.light_dir, this.light_dir, this.light_dir);
			Light component3 = GameObject.Find("Directional light2").GetComponent<Light>();
			component3.transform.rotation = Quaternion.Euler(-this.light_dir, -this.light_dir, -this.light_dir);
			GUILayout.Label("Model orientation (snow)", new GUILayoutOption[]
			{
				GUILayout.MaxWidth(170f)
			});
			Transform transform = GameObject.Find("WeirdOne").transform;
			this.model_dir = GUILayout.HorizontalSlider(this.model_dir, 0f, 180f, new GUILayoutOption[0]);
			transform.rotation = Quaternion.Euler(this.model_dir, this.model_dir * 0.7f, -this.model_dir * 0.1f);
			if (!Application.isWebPlayer && GUILayout.Button("QUIT", new GUILayoutOption[0]))
			{
				Application.Quit();
			}
			GUILayout.Label("  F (hold) - freeze camera", new GUILayoutOption[0]);
			GUILayout.Label("  ,/. - zoom camera", new GUILayoutOption[0]);
		}
		else if (!Application.isWebPlayer && GUILayout.Button("QUIT", new GUILayoutOption[0]))
		{
			Application.Quit();
		}
		GUILayout.Label("  P - toggle panel", new GUILayoutOption[0]);
		GUILayout.EndVertical();
	}
}
