using System;
using UnityEngine;

public class AutoMobileShaderSwitch : MonoBehaviour
{
	[Serializable]
	public class ReplacementDefinition
	{
		public Shader original;

		public Shader replacement;
	}

	[Serializable]
	public class ReplacementList
	{
		public AutoMobileShaderSwitch.ReplacementDefinition[] items = new AutoMobileShaderSwitch.ReplacementDefinition[0];
	}

	[SerializeField]
	private AutoMobileShaderSwitch.ReplacementList replacements;

	private void OnEnable()
	{
	}
}
