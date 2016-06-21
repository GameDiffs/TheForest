using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ceto
{
	public class RefractionCommand : IRefractionCommand
	{
		public class Command
		{
			public CommandBuffer buffer;

			public Material material;

			public bool enabled;

			public CameraEvent camEvent;
		}

		private Camera m_camera;

		private RefractionCommand.Command CopyScreenCmd
		{
			get;
			set;
		}

		private RefractionCommand.Command CopyDepthCmd
		{
			get;
			set;
		}

		private RefractionCommand.Command NormalFadeCmd
		{
			get;
			set;
		}

		public bool DisableCopyDepthCmd
		{
			get
			{
				return this.CopyDepthCmd.enabled;
			}
			set
			{
				this.CopyDepthCmd.enabled = !value;
			}
		}

		public bool DisableNormalFadeCmd
		{
			get
			{
				return this.NormalFadeCmd.enabled;
			}
			set
			{
				this.NormalFadeCmd.enabled = !value;
			}
		}

		public RefractionCommand(Camera cam, Shader copyDepth, Shader normalFade)
		{
			this.CopyScreenCmd = new RefractionCommand.Command();
			this.CopyDepthCmd = new RefractionCommand.Command();
			this.NormalFadeCmd = new RefractionCommand.Command();
			this.m_camera = cam;
			this.CopyDepthCmd.material = new Material(copyDepth);
			this.CopyDepthCmd.enabled = true;
			this.NormalFadeCmd.material = new Material(normalFade);
			this.NormalFadeCmd.enabled = true;
		}

		public void ClearCommands()
		{
			if (this.CopyScreenCmd.buffer != null)
			{
				this.m_camera.RemoveCommandBuffer(this.CopyScreenCmd.camEvent, this.CopyScreenCmd.buffer);
				this.CopyScreenCmd.buffer = null;
			}
			if (this.CopyDepthCmd.buffer != null)
			{
				this.m_camera.RemoveCommandBuffer(this.CopyDepthCmd.camEvent, this.CopyDepthCmd.buffer);
				this.CopyDepthCmd.buffer = null;
			}
			if (this.NormalFadeCmd.buffer != null)
			{
				this.m_camera.RemoveCommandBuffer(this.NormalFadeCmd.camEvent, this.NormalFadeCmd.buffer);
				this.NormalFadeCmd.buffer = null;
			}
		}

		public void UpdateCommands()
		{
			RenderingPath actualRenderingPath = this.m_camera.actualRenderingPath;
			CameraEvent cameraEvent;
			CameraEvent cameraEvent2;
			CameraEvent cameraEvent3;
			int pass;
			if (actualRenderingPath == RenderingPath.DeferredShading)
			{
				cameraEvent = CameraEvent.AfterSkybox;
				cameraEvent2 = CameraEvent.AfterLighting;
				cameraEvent3 = CameraEvent.AfterLighting;
				pass = 1;
			}
			else if (actualRenderingPath == RenderingPath.DeferredLighting)
			{
				cameraEvent = CameraEvent.AfterSkybox;
				cameraEvent2 = CameraEvent.AfterLighting;
				cameraEvent3 = CameraEvent.AfterLighting;
				pass = 0;
			}
			else
			{
				cameraEvent = CameraEvent.AfterSkybox;
				cameraEvent2 = CameraEvent.AfterDepthTexture;
				cameraEvent3 = CameraEvent.AfterDepthNormalsTexture;
				pass = 0;
			}
			if ((!this.CopyScreenCmd.enabled || this.CopyScreenCmd.camEvent != cameraEvent) && this.CopyScreenCmd.buffer != null)
			{
				this.m_camera.RemoveCommandBuffer(this.CopyScreenCmd.camEvent, this.CopyScreenCmd.buffer);
				this.CopyScreenCmd.buffer = null;
			}
			if ((!this.CopyDepthCmd.enabled || this.CopyDepthCmd.camEvent != cameraEvent2) && this.CopyDepthCmd.buffer != null)
			{
				this.m_camera.RemoveCommandBuffer(this.CopyDepthCmd.camEvent, this.CopyDepthCmd.buffer);
				this.CopyDepthCmd.buffer = null;
			}
			if ((!this.NormalFadeCmd.enabled || this.NormalFadeCmd.camEvent != cameraEvent3) && this.NormalFadeCmd.buffer != null)
			{
				this.m_camera.RemoveCommandBuffer(this.NormalFadeCmd.camEvent, this.NormalFadeCmd.buffer);
				this.NormalFadeCmd.buffer = null;
			}
			if (this.CopyScreenCmd.enabled && this.CopyScreenCmd.buffer == null)
			{
				RenderTextureFormat format;
				if (this.m_camera.hdr)
				{
					format = RenderTextureFormat.ARGBHalf;
				}
				else
				{
					format = RenderTextureFormat.ARGB32;
				}
				if (!SystemInfo.SupportsRenderTextureFormat(format))
				{
					format = RenderTextureFormat.ARGB32;
				}
				CommandBuffer commandBuffer = new CommandBuffer();
				commandBuffer.name = "Ceto CopyScreen Cmd: " + this.m_camera.name;
				int nameID = Shader.PropertyToID("Ceto_CopyScreenTexture_Tmp");
				commandBuffer.GetTemporaryRT(nameID, -1, -1, 0, FilterMode.Bilinear, format, RenderTextureReadWrite.Default);
				commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, nameID);
				commandBuffer.SetGlobalTexture(Ocean.REFRACTION_GRAB_TEXTURE_NAME, nameID);
				this.CopyScreenCmd.buffer = commandBuffer;
				this.CopyScreenCmd.camEvent = cameraEvent;
				this.m_camera.AddCommandBuffer(this.CopyScreenCmd.camEvent, this.CopyScreenCmd.buffer);
			}
			if (this.CopyDepthCmd.enabled && this.CopyDepthCmd.buffer == null)
			{
				RenderTextureFormat format2 = RenderTextureFormat.RFloat;
				if (!SystemInfo.SupportsRenderTextureFormat(format2))
				{
					format2 = RenderTextureFormat.RHalf;
				}
				if (!SystemInfo.SupportsRenderTextureFormat(format2))
				{
					format2 = RenderTextureFormat.ARGBHalf;
				}
				CommandBuffer commandBuffer2 = new CommandBuffer();
				commandBuffer2.name = "Ceto Copy Depth Cmd: " + this.m_camera.name;
				int nameID2 = Shader.PropertyToID("Ceto_DepthCopyTexture_Tmp");
				commandBuffer2.GetTemporaryRT(nameID2, -1, -1, 0, FilterMode.Point, format2, RenderTextureReadWrite.Linear);
				commandBuffer2.Blit(BuiltinRenderTextureType.CurrentActive, nameID2, this.CopyDepthCmd.material, 0);
				commandBuffer2.SetGlobalTexture(Ocean.DEPTH_GRAB_TEXTURE_NAME, nameID2);
				this.CopyDepthCmd.buffer = commandBuffer2;
				this.CopyDepthCmd.camEvent = cameraEvent2;
				this.m_camera.AddCommandBuffer(this.CopyDepthCmd.camEvent, this.CopyDepthCmd.buffer);
			}
			if (this.NormalFadeCmd.enabled && this.NormalFadeCmd.buffer == null)
			{
				RenderTextureFormat format3 = RenderTextureFormat.R8;
				if (!SystemInfo.SupportsRenderTextureFormat(format3))
				{
					format3 = RenderTextureFormat.ARGB32;
				}
				CommandBuffer commandBuffer3 = new CommandBuffer();
				commandBuffer3.name = "Ceto Normal Fade Cmd: " + this.m_camera.name;
				int nameID3 = Shader.PropertyToID("Ceto_NormalFadeTexture_Tmp");
				commandBuffer3.GetTemporaryRT(nameID3, -1, -1, 0, FilterMode.Bilinear, format3, RenderTextureReadWrite.Linear);
				commandBuffer3.Blit(BuiltinRenderTextureType.CurrentActive, nameID3, this.NormalFadeCmd.material, pass);
				commandBuffer3.SetGlobalTexture(Ocean.NORMAL_FADE_TEXTURE_NAME, nameID3);
				this.NormalFadeCmd.buffer = commandBuffer3;
				this.NormalFadeCmd.camEvent = cameraEvent3;
				this.m_camera.AddCommandBuffer(this.NormalFadeCmd.camEvent, this.NormalFadeCmd.buffer);
			}
			if (this.NormalFadeCmd.buffer != null)
			{
				this.NormalFadeCmd.material.SetMatrix("Ceto_NormalFade_MV", this.m_camera.cameraToWorldMatrix);
			}
		}
	}
}
