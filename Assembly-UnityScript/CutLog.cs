using System;
using UnityEngine;

[Serializable]
public class CutLog : MonoBehaviour
{
	public GameObject MyTree;

	public GameObject CutTree;

	private GameObject Player;

	private bool PlayerHere;

	public override void OnTriggerEnter(Collider otherObject)
	{
		if (otherObject.gameObject.CompareTag("Player"))
		{
			this.Player = otherObject.gameObject;
			this.PlayerHere = true;
			otherObject.SendMessage("ShowCutLog");
		}
	}

	public override void Update()
	{
		if (this.PlayerHere && Input.GetButtonDown("Take"))
		{
			this.CutTree.SetActive(true);
			this.CutTree.transform.parent = null;
			this.Player.SendMessage("PlayAxeHit");
			this.Player.SendMessage("CloseCutLog");
			this.MyTree.SetActive(false);
		}
	}

	public override void OnTriggerExit(Collider otherObject)
	{
		if (otherObject.gameObject.CompareTag("Player"))
		{
			otherObject.SendMessage("CloseCutLog");
			this.PlayerHere = false;
		}
	}

	public override void Main()
	{
	}
}
