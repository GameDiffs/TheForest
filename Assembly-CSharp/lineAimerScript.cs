using System;
using UnityEngine;

public class lineAimerScript : MonoBehaviour
{
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);
	}
}
