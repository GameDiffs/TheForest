using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Camera), HutongGames.PlayMaker.Tooltip("Activates a Camera in the scene.")]
	public class CutToCamera : FsmStateAction
	{
		[RequiredField]
		public Camera camera;

		public bool makeMainCamera;

		public bool cutBackOnExit;

		private Camera oldCamera;

		public override void Reset()
		{
			this.camera = null;
			this.makeMainCamera = true;
			this.cutBackOnExit = false;
		}

		public override void OnEnter()
		{
			if (this.camera == null)
			{
				this.LogError("Missing camera!");
				return;
			}
			this.oldCamera = Camera.main;
			CutToCamera.SwitchCamera(Camera.main, this.camera);
			if (this.makeMainCamera)
			{
				this.camera.tag = "MainCamera";
			}
			base.Finish();
		}

		public override void OnExit()
		{
			if (this.cutBackOnExit)
			{
				CutToCamera.SwitchCamera(this.camera, this.oldCamera);
			}
		}

		private static void SwitchCamera(Camera camera1, Camera camera2)
		{
			if (camera1 != null)
			{
				camera1.enabled = false;
			}
			if (camera2 != null)
			{
				camera2.enabled = true;
			}
		}
	}
}
