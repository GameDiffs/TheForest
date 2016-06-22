using System;
using UnityEngine;

[Serializable]
public class PickUpAuto : MonoBehaviour
{
	public bool Tooth;

	public bool Leaf;

	public bool PlaneAxe;

	public PickUpAuto()
	{
		this.Leaf = true;
	}

	public override void OnTriggerEnter(Collider otherObject)
	{
		if (otherObject.gameObject.CompareTag("Player"))
		{
			if (this.Tooth)
			{
				otherObject.SendMessage("GotTooth");
			}
			if (this.Leaf)
			{
				otherObject.SendMessage("GotLeaf");
			}
			if (this.PlaneAxe)
			{
				otherObject.SendMessage("GotTornMetal");
			}
			UnityEngine.Object.Destroy(this.transform.parent.gameObject);
		}
	}

	public override void Main()
	{
	}
}
