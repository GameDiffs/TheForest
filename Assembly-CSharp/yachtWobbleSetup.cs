using System;
using UnityEngine;

public class yachtWobbleSetup : MonoBehaviour
{
	private void Start()
	{
		base.Invoke("setupWobble", 1f);
	}

	private void setupWobble()
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate((GameObject)Resources.Load("CutScene/yachtWobblePrefab"), base.transform.position, Quaternion.identity);
		Vector3 position = base.transform.position + base.transform.up * -30f;
		gameObject.transform.position = position;
		gameObject.transform.parent = base.transform.parent;
		base.transform.parent = gameObject.GetComponentInChildren<yachtAnimSetup>().transform;
	}
}
