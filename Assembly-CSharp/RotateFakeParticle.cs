using System;
using TheForest.Utils;
using UnityEngine;

public class RotateFakeParticle : MonoBehaviour
{
	private void Update()
	{
		Vector3 vector = base.transform.position - LocalPlayer.Transform.position;
		vector.y = 0f;
		base.transform.rotation = Quaternion.Euler(0f, Vector3.Angle(Vector3.forward, vector) * (float)((Vector3.Cross(Vector3.forward, vector).y >= 0f) ? 1 : -1), 0f);
	}
}
