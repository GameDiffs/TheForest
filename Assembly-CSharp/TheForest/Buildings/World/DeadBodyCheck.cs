using System;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/World/Dead Body Check")]
	public class DeadBodyCheck : MonoBehaviour
	{
		public GameObject _receiver;

		private void OnTriggerEnter(Collider other)
		{
			if (other.GetComponent<mutantPickUp>())
			{
				this._receiver.SendMessage("DeadBodyEnteredArea");
			}
		}
	}
}
