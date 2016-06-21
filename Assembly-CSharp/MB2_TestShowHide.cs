using System;
using UnityEngine;

public class MB2_TestShowHide : MonoBehaviour
{
	public MB3_MeshBaker mb;

	public GameObject[] objs;

	private void Update()
	{
		if (Time.frameCount == 100)
		{
			this.mb.ShowHide(null, this.objs);
			this.mb.ApplyShowHide();
			Debug.Log("should have disappeared");
		}
		if (Time.frameCount == 200)
		{
			this.mb.ShowHide(this.objs, null);
			this.mb.ApplyShowHide();
			Debug.Log("should show");
		}
	}
}
