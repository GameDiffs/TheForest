using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	public class RenderTexureUtility
	{
		private List<RenderTexture> m_TemporaryRTs = new List<RenderTexture>();

		public RenderTexture GetTemporaryRenderTexture(int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.ARGBHalf, FilterMode filterMode = FilterMode.Bilinear)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, depthBuffer, format);
			temporary.filterMode = filterMode;
			temporary.wrapMode = TextureWrapMode.Clamp;
			temporary.name = "RenderTextureUtilityTempTexture";
			this.m_TemporaryRTs.Add(temporary);
			return temporary;
		}

		public void ReleaseTemporaryRenderTexture(RenderTexture rt)
		{
			if (rt == null)
			{
				return;
			}
			if (!this.m_TemporaryRTs.Contains(rt))
			{
				Debug.LogErrorFormat("Attempting to remove texture that was not allocated: {0}", new object[]
				{
					rt
				});
				return;
			}
			this.m_TemporaryRTs.Remove(rt);
			RenderTexture.ReleaseTemporary(rt);
		}

		public void ReleaseAllTemporyRenderTexutres()
		{
			for (int i = 0; i < this.m_TemporaryRTs.Count; i++)
			{
				RenderTexture.ReleaseTemporary(this.m_TemporaryRTs[i]);
			}
			this.m_TemporaryRTs.Clear();
		}
	}
}
