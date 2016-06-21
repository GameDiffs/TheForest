using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Player
{
	[DoNotSerializePublic]
	public class PassengerView : MonoBehaviour
	{
		public int _id;

		public bool _standalone;

		private void OnDisable()
		{
			this._id = -1;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (this._id >= 0 && other.CompareTag("Grabber"))
			{
				LocalPlayer.PassengerManifest.FoundPassenger(this._id);
				base.GetComponent<Collider>().enabled = false;
			}
		}
	}
}
