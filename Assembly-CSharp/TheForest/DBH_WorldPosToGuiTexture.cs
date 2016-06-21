using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest
{
	public class DBH_WorldPosToGuiTexture : MonoBehaviour
	{
		public GUIText _text;

		private void Awake()
		{
			this._text = new GameObject("WorldPosText").AddComponent<GUIText>();
		}

		private void OnDestroy()
		{
			if (this._text)
			{
				UnityEngine.Object.Destroy(this._text.gameObject);
			}
		}

		private void LateUpdate()
		{
			if (this._text)
			{
				this._text.text = string.Concat(new object[]
				{
					base.transform.position.x,
					"\n",
					base.transform.position.y,
					"\n",
					base.transform.position.z
				});
				Vector3 a = LocalPlayer.MainCamTr.InverseTransformPoint(base.transform.position);
				a.z = Mathf.Max(a.z, 0.1f);
				this._text.transform.position = LocalPlayer.MainCam.WorldToViewportPoint(LocalPlayer.MainCamTr.TransformPoint(a + Vector3.up));
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
