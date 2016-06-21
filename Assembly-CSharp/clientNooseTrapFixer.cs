using System;
using UnityEngine;

public class clientNooseTrapFixer : MonoBehaviour
{
	public Transform nooseFootPivot;

	public GameObject sprungRopeGo;

	public GameObject looseRopeGo;

	public GameObject sprungNooseJoint;

	private void Start()
	{
		if (!BoltNetwork.isClient)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("enemyRoot"))
		{
			other.SendMessage("setClientTrigger", base.gameObject, SendMessageOptions.DontRequireReceiver);
			other.SendMessage("setClientNoosePivot", this.nooseFootPivot, SendMessageOptions.DontRequireReceiver);
		}
	}
}
