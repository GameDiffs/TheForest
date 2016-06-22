using System;
using UnityEngine;

[Serializable]
public class MissionScriptflare : MonoBehaviour
{
	public bool FlareTrigger;

	public bool IslandTrigger;

	public bool HallTrigger;

	public GameObject MyObject;

	public override void OnTriggerEnter(Collider otherObject)
	{
		if (otherObject.gameObject.CompareTag("Player"))
		{
			if (this.FlareTrigger)
			{
				this.MyObject.SetActive(true);
				UnityEngine.Object.Destroy(this.gameObject);
			}
			if (this.HallTrigger)
			{
				this.MyObject.SendMessage("SwitchTarget");
				UnityEngine.Object.Destroy(this.gameObject);
			}
			if (this.IslandTrigger)
			{
				otherObject.SendMessage("DrawRaft");
				UnityEngine.Object.Destroy(this.gameObject);
			}
		}
	}

	public override void Main()
	{
	}
}
