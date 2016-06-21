using System;
using UnityEngine;

namespace ScionEngine
{
	public static class ShaderSettings
	{
		public class IndexOption
		{
			private int curValue = -1;

			private string[] keywords;

			public IndexOption(string[] _keywords)
			{
				this.keywords = _keywords;
				this.curValue = -1;
			}

			public void SetIndex(int index)
			{
				if (index != this.curValue)
				{
					this.SetKeyword(index);
					this.curValue = index;
				}
			}

			public bool IsActive(int index)
			{
				return this.curValue == index;
			}

			public bool IsActive(string keyword)
			{
				for (int i = 0; i < this.keywords.Length; i++)
				{
					if (keyword == this.keywords[i])
					{
						return this.IsActive(i);
					}
				}
				return false;
			}

			private void SetKeyword(int index)
			{
				for (int i = 0; i < this.keywords.Length; i++)
				{
					if (i == index)
					{
						Shader.EnableKeyword(this.keywords[i]);
					}
					else
					{
						Shader.DisableKeyword(this.keywords[i]);
					}
				}
			}

			private void DisableKeyword(int index)
			{
				Shader.DisableKeyword(this.keywords[index]);
			}
		}

		public const string ChromaticAberrationOffKW = "SC_CHROMATIC_ABERRATION_OFF";

		public const string ChromaticAberrationOnKW = "SC_CHROMATIC_ABERRATION_ON";

		public const string ColorGradingOffKW = "SC_COLOR_CORRECTION_OFF";

		public const string ColorGradingOn1TexKW = "SC_COLOR_CORRECTION_1_TEX";

		private static readonly string[] ChromaticAberrationKeywords = new string[]
		{
			"SC_CHROMATIC_ABERRATION_OFF",
			"SC_CHROMATIC_ABERRATION_ON"
		};

		public static ShaderSettings.IndexOption ChromaticAberrationSettings = new ShaderSettings.IndexOption(ShaderSettings.ChromaticAberrationKeywords);

		private static readonly string[] ColorGradingKeywords = new string[]
		{
			"SC_COLOR_CORRECTION_OFF",
			"SC_COLOR_CORRECTION_1_TEX"
		};

		public static ShaderSettings.IndexOption ColorGradingSettings = new ShaderSettings.IndexOption(ShaderSettings.ColorGradingKeywords);
	}
}
