using System;
using UnityEngine;

public class FindPlanePeople : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Grabber"))
		{
			other.transform.root.SendMessage("RecordedBody");
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
