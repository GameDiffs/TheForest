using System;
using UnityEngine;

namespace TheForest.Player
{
	[DoNotSerializePublic, RequireComponent(typeof(GreebleZone))]
	public class PassengerViewZone : MonoBehaviour
	{
		public int _zoneId;

		public int[] _passengerIds;

		private void Awake()
		{
			base.GetComponent<GreebleZone>().OnSpawned = new Action<int, GameObject>(this.OnPassengerSpawned);
		}

		public void OnPassengerSpawned(int index, GameObject go)
		{
			if (index >= 0 && index < this._passengerIds.Length)
			{
				PassengerView component = go.GetComponent<PassengerView>();
				if (component)
				{
					component._id = this._passengerIds[index];
					go.GetComponent<Collider>().enabled = true;
				}
			}
		}
	}
}
