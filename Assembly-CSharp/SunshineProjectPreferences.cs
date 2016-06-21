using System;
using UnityEngine;

public class SunshineProjectPreferences : ScriptableObject
{
	public const string AssetPath = "Assets/Sunshine/Resources/SunshinePreferences.asset";

	public const string ResourceName = "SunshinePreferences";

	private static SunshineProjectPreferences instance;

	[SerializeField]
	private string version = string.Empty;

	[SerializeField]
	private bool forwardShadersInstalled;

	[SerializeField]
	private bool useCustomShadows;

	[SerializeField]
	private bool manualShaderInstallation = true;

	[SerializeField]
	private bool installerRunning;

	public static SunshineProjectPreferences Instance
	{
		get
		{
			if (SunshineProjectPreferences.instance == null)
			{
				SunshineProjectPreferences.instance = SunshineProjectPreferences.Load();
			}
			return SunshineProjectPreferences.instance;
		}
	}

	public string Version
	{
		get
		{
			return this.version;
		}
		set
		{
			if (this.version == value)
			{
				return;
			}
			this.version = value;
		}
	}

	public bool ForwardShadersInstalled
	{
		get
		{
			return this.forwardShadersInstalled;
		}
		set
		{
			if (this.forwardShadersInstalled == value)
			{
				return;
			}
			this.forwardShadersInstalled = value;
		}
	}

	public bool UseCustomShadows
	{
		get
		{
			return this.useCustomShadows;
		}
		set
		{
			if (this.useCustomShadows == value)
			{
				return;
			}
			this.useCustomShadows = value;
		}
	}

	public bool ManualShaderInstallation
	{
		get
		{
			return this.manualShaderInstallation;
		}
		set
		{
			if (this.manualShaderInstallation == value)
			{
				return;
			}
			this.manualShaderInstallation = value;
		}
	}

	public bool InstallerRunning
	{
		get
		{
			return this.installerRunning;
		}
		set
		{
			if (this.installerRunning == value)
			{
				return;
			}
			this.installerRunning = value;
		}
	}

	private static SunshineProjectPreferences Load()
	{
		SunshineProjectPreferences sunshineProjectPreferences = null;
		try
		{
			sunshineProjectPreferences = (Resources.Load("SunshinePreferences", typeof(SunshineProjectPreferences)) as SunshineProjectPreferences);
		}
		catch
		{
		}
		if (sunshineProjectPreferences == null)
		{
			sunshineProjectPreferences = ScriptableObject.CreateInstance<SunshineProjectPreferences>();
			sunshineProjectPreferences.name = "Sunshine Project Configuration";
			sunshineProjectPreferences.hideFlags = HideFlags.NotEditable;
		}
		return sunshineProjectPreferences;
	}

	public void SaveIfDirty()
	{
	}
}
