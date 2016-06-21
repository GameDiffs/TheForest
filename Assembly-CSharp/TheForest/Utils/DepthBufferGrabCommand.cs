using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TheForest.Utils
{
	[RequireComponent(typeof(Camera))]
	public class DepthBufferGrabCommand : MonoBehaviour
	{
		public class CommandData
		{
			public HashSet<string> bindings = new HashSet<string>();

			public CommandBuffer command;

			public int width;

			public int height;

			public bool recreate;
		}

		public Shader depthCopySdr;

		private static Dictionary<Camera, DepthBufferGrabCommand.CommandData> m_data = new Dictionary<Camera, DepthBufferGrabCommand.CommandData>();

		private Camera m_camera;

		private Material m_depthCopyMat;

		private void Start()
		{
			this.m_camera = base.GetComponent<Camera>();
			this.m_depthCopyMat = new Material(this.depthCopySdr);
			if (!DepthBufferGrabCommand.m_data.ContainsKey(this.m_camera))
			{
				DepthBufferGrabCommand.m_data.Add(this.m_camera, new DepthBufferGrabCommand.CommandData());
			}
			this.CreateCommand();
		}

		private void Update()
		{
			this.CreateCommand();
		}

		public static void AddBinding(Camera cam, string name)
		{
			if (DepthBufferGrabCommand.m_data == null || !DepthBufferGrabCommand.m_data.ContainsKey(cam))
			{
				return;
			}
			DepthBufferGrabCommand.CommandData commandData = DepthBufferGrabCommand.m_data[cam];
			if (commandData.bindings.Contains(name))
			{
				return;
			}
			commandData.bindings.Add(name);
			commandData.recreate = true;
		}

		public static void RemoveBinding(Camera cam, string name)
		{
			if (DepthBufferGrabCommand.m_data == null || !DepthBufferGrabCommand.m_data.ContainsKey(cam))
			{
				return;
			}
			DepthBufferGrabCommand.CommandData commandData = DepthBufferGrabCommand.m_data[cam];
			if (!commandData.bindings.Contains(name))
			{
				return;
			}
			commandData.bindings.Remove(name);
			commandData.recreate = true;
		}

		public static bool HasCamera(Camera cam)
		{
			return DepthBufferGrabCommand.m_data != null && DepthBufferGrabCommand.m_data.ContainsKey(cam);
		}

		private void CreateCommand()
		{
			if (DepthBufferGrabCommand.m_data == null || !DepthBufferGrabCommand.m_data.ContainsKey(this.m_camera))
			{
				return;
			}
			DepthBufferGrabCommand.CommandData commandData = DepthBufferGrabCommand.m_data[this.m_camera];
			if (commandData.command == null && commandData.bindings.Count == 0)
			{
				return;
			}
			if (commandData.command != null && commandData.bindings.Count == 0)
			{
				this.m_camera.RemoveCommandBuffer(CameraEvent.AfterSkybox, commandData.command);
				commandData.command = null;
				return;
			}
			if (!commandData.recreate && commandData.command != null && commandData.width == this.m_camera.pixelWidth && commandData.height == this.m_camera.pixelHeight)
			{
				return;
			}
			if (commandData.command != null)
			{
				this.m_camera.RemoveCommandBuffer(CameraEvent.AfterSkybox, commandData.command);
			}
			commandData.command = new CommandBuffer();
			commandData.command.name = "Shared Depth Grab Cmd: " + this.m_camera.name;
			commandData.width = this.m_camera.pixelWidth;
			commandData.height = this.m_camera.pixelHeight;
			int nameID = Shader.PropertyToID("_DepthCopyTexture");
			commandData.command.GetTemporaryRT(nameID, commandData.width, commandData.height, 0, FilterMode.Point, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
			commandData.command.Blit(BuiltinRenderTextureType.CurrentActive, nameID, this.m_depthCopyMat, 0);
			HashSet<string>.Enumerator enumerator = commandData.bindings.GetEnumerator();
			while (enumerator.MoveNext())
			{
				commandData.command.SetGlobalTexture(enumerator.Current, nameID);
			}
			this.m_camera.AddCommandBuffer(CameraEvent.AfterSkybox, commandData.command);
			commandData.recreate = false;
		}
	}
}
