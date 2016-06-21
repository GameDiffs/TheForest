using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.UI
{
	public class UiFollowTarget : MonoBehaviour
	{
		public Transform _target;

		public Transform _target2;

		public Vector3 _worldOffset;

		public float _minDepth = 1.45f;

		public Vector3 _worldOffsetBook;

		public float _depthRatioBook = 5f;

		public bool _inBook;

		private void LateUpdate()
		{
			Vector3 position;
			if (!this._inBook)
			{
				position = LocalPlayer.MainCam.WorldToViewportPoint(this._target.position + this._worldOffset);
				if (this._target2)
				{
					Vector3 vector = LocalPlayer.MainCam.WorldToViewportPoint(this._target2.position + this._worldOffset);
					if (vector.z > 0.25f && (vector.z < position.z || position.z < 0.5f || position.x < 0f || position.x > 1f || position.y < 0f || position.y > 1f))
					{
						position = vector;
					}
				}
				if (position.z > 0f)
				{
					position.z = Mathf.Max(position.z, this._minDepth);
				}
			}
			else
			{
				position = LocalPlayer.MainCam.WorldToViewportPoint(this._target.position + this._worldOffsetBook);
				position.z *= this._depthRatioBook;
			}
			base.transform.position = Scene.HudGui.ActionIconCams.ViewportToWorldPoint(position);
		}
	}
}
