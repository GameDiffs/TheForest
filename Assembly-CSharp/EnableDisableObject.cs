using System;
using UnityEngine;

[ExecuteInEditMode]
public class EnableDisableObject : MonoBehaviour
{
	private bool Enabled = true;

	public Light Light1;

	public Light Light2;

	private void Start()
	{
	}

	private void Update()
	{
		Behaviour arg_1A_0 = this.Light1;
		bool enabled = this.Enabled;
		this.Light2.enabled = enabled;
		arg_1A_0.enabled = enabled;
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(185f, 145f, 150f, 30f), "Turn lights on-off");
		this.Enabled = GUI.Toggle(new Rect(295f, 145f, 100f, 20f), this.Enabled, string.Empty);
	}
}
