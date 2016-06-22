using System;
using UnityEngine;

[Serializable]
public class ElementDamage : MonoBehaviour
{
	public override void OnTriggerEnter(Collider otherObject)
	{
		if (!this.transform.root.Equals(otherObject.transform.root))
		{
			otherObject.SendMessage("Burn", SendMessageOptions.DontRequireReceiver);
		}
	}

	public override void Main()
	{
	}
}
