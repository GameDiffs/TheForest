using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.World
{
	public class CompassNeedle : MonoBehaviour
	{
		public bool _usePlayerForward = true;

		private void Update()
		{
			float num;
			if (this._usePlayerForward)
			{
				num = Vector3.Angle(LocalPlayer.Transform.forward, Vector3.back);
				if (Vector3.Cross(LocalPlayer.Transform.forward, Vector3.back).y < 0f)
				{
					num = -num;
				}
			}
			else
			{
				num = Vector3.Angle(base.transform.forward, Vector3.back);
				if (Vector3.Cross(base.transform.forward, Vector3.back).y < 0f)
				{
					num = -num;
				}
			}
			base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, Quaternion.Euler(0f, num, 0f), Time.deltaTime * 2.5f);
		}
	}
}
