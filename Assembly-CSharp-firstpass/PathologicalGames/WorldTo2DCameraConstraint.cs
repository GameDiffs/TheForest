using System;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/UnityConstraints/Constraint- World To 2D Camera")]
	public class WorldTo2DCameraConstraint : TransformConstraint
	{
		public enum OFFSET_MODE
		{
			WorldSpace,
			ViewportSpace
		}

		public enum OFFSCREEN_MODE
		{
			Constrain,
			Limit,
			DoNothing
		}

		public bool vertical = true;

		public Camera targetCamera;

		public Camera orthoCamera;

		public Vector3 offset;

		public WorldTo2DCameraConstraint.OFFSET_MODE offsetMode;

		public WorldTo2DCameraConstraint.OFFSCREEN_MODE offScreenMode;

		public Vector2 offscreenThreasholdW = new Vector2(0f, 1f);

		public Vector2 offscreenThreasholdH = new Vector2(0f, 1f);

		protected override void Awake()
		{
			base.Awake();
			Camera[] array = UnityEngine.Object.FindObjectsOfType(typeof(Camera)) as Camera[];
			Camera[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Camera camera = array2[i];
				if (camera.orthographic)
				{
					if ((camera.cullingMask & 1 << base.gameObject.layer) > 0)
					{
						this.orthoCamera = camera;
						break;
					}
				}
			}
			if (base.target != null)
			{
				Camera[] array3 = array;
				for (int j = 0; j < array3.Length; j++)
				{
					Camera camera2 = array3[j];
					if ((camera2.cullingMask & 1 << base.target.gameObject.layer) > 0)
					{
						this.targetCamera = camera2;
						break;
					}
				}
			}
		}

		protected override void OnConstraintUpdate()
		{
			bool constrainPosition = this.constrainPosition;
			this.constrainPosition = false;
			base.OnConstraintUpdate();
			this.constrainPosition = constrainPosition;
			if (!this.constrainPosition)
			{
				return;
			}
			this.pos = base.target.position;
			if (this.offsetMode == WorldTo2DCameraConstraint.OFFSET_MODE.WorldSpace)
			{
				this.pos.x = this.pos.x + this.offset.x;
				this.pos.y = this.pos.y + this.offset.y;
			}
			this.pos = this.targetCamera.WorldToViewportPoint(this.pos);
			if (this.offsetMode == WorldTo2DCameraConstraint.OFFSET_MODE.ViewportSpace)
			{
				this.pos.x = this.pos.x + this.offset.x;
				this.pos.y = this.pos.y + this.offset.y;
			}
			switch (this.offScreenMode)
			{
			case WorldTo2DCameraConstraint.OFFSCREEN_MODE.Limit:
				this.pos.x = Mathf.Clamp(this.pos.x, this.offscreenThreasholdW.x, this.offscreenThreasholdW.y);
				this.pos.y = Mathf.Clamp(this.pos.y, this.offscreenThreasholdH.x, this.offscreenThreasholdH.y);
				break;
			case WorldTo2DCameraConstraint.OFFSCREEN_MODE.DoNothing:
				if (this.pos.z <= 0f || this.pos.x <= this.offscreenThreasholdW.x || this.pos.x >= this.offscreenThreasholdW.y || this.pos.y <= this.offscreenThreasholdH.x || this.pos.y >= this.offscreenThreasholdH.y)
				{
					return;
				}
				break;
			}
			this.pos = this.orthoCamera.ViewportToWorldPoint(this.pos);
			this.pos.z = this.offset.z;
			if (!this.outputPosX)
			{
				this.pos.x = base.position.x;
			}
			if (!this.outputPosY)
			{
				this.pos.y = base.position.y;
			}
			if (!this.outputPosZ)
			{
				this.pos.z = base.position.z;
			}
			this.xform.position = this.pos;
		}
	}
}
