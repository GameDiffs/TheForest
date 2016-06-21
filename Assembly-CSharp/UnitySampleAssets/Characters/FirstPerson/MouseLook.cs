using System;
using TheForest.Utils;
using UnityEngine;

namespace UnitySampleAssets.Characters.FirstPerson
{
	[Serializable]
	public class MouseLook
	{
		public float XSensitivity = 5f;

		public float YSensitivity = 5f;

		public bool clampVerticalRotation = true;

		public float MinimumX = -90f;

		public float MaximumX = 90f;

		public bool smooth;

		public float smoothTime = 5f;

		private Quaternion m_CharacterTargetRot;

		private Quaternion m_CameraTargetRot;

		public void Init(Transform character, Transform camera)
		{
			this.m_CharacterTargetRot = character.localRotation;
			this.m_CameraTargetRot = camera.localRotation;
		}

		public void LookRotation(Transform character, Transform camera)
		{
			float y = TheForest.Utils.Input.GetAxis("Mouse X") * this.XSensitivity * (PlayerPreferences.MouseSensitivity + 0.5f);
			float num = TheForest.Utils.Input.GetAxis("Mouse Y") * this.YSensitivity * (PlayerPreferences.MouseSensitivity + 0.5f);
			if (PlayerPreferences.MouseInvert)
			{
				num = -num;
			}
			if (!LocalPlayer.FpCharacter.PushingSled)
			{
				this.m_CharacterTargetRot *= Quaternion.Euler(0f, y, 0f);
			}
			this.m_CameraTargetRot *= Quaternion.Euler(-num, 0f, 0f);
			if (this.clampVerticalRotation)
			{
				this.m_CameraTargetRot = this.ClampRotationAroundXAxis(this.m_CameraTargetRot);
			}
			if (this.smooth)
			{
				character.localRotation = Quaternion.Slerp(character.localRotation, this.m_CharacterTargetRot, this.smoothTime * Time.deltaTime);
				camera.localRotation = Quaternion.Slerp(camera.localRotation, this.m_CameraTargetRot, this.smoothTime * Time.deltaTime);
			}
			else
			{
				character.localRotation = this.m_CharacterTargetRot;
				camera.localRotation = this.m_CameraTargetRot;
			}
		}

		private Quaternion ClampRotationAroundXAxis(Quaternion q)
		{
			q.x /= q.w;
			q.y /= q.w;
			q.z /= q.w;
			q.w = 1f;
			float num = 114.59156f * Mathf.Atan(q.x);
			num = Mathf.Clamp(num, this.MinimumX, this.MaximumX);
			q.x = Mathf.Tan(0.008726646f * num);
			return q;
		}
	}
}
