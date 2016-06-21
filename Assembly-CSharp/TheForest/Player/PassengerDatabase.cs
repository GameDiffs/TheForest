using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace TheForest.Player
{
	public class PassengerDatabase : ScriptableObject
	{
		public List<Passenger> _passengers = new List<Passenger>();

		[HideInInspector]
		public int _passengersZonesMaxID;

		private Dictionary<int, Passenger> _passengersCache;

		public static PassengerDatabase Instance
		{
			get;
			private set;
		}

		public void OnEnable()
		{
			base.hideFlags = HideFlags.None;
			if (PassengerDatabase.Instance == null)
			{
				PassengerDatabase.Instance = this;
				this._passengersCache = this._passengers.ToDictionary((Passenger p) => p._id);
			}
		}

		public string GetPassengerName(int passengerId)
		{
			return (!this._passengersCache.ContainsKey(passengerId)) ? null : this._passengersCache[passengerId]._name;
		}

		public int GetPassengerNum(int passengerId)
		{
			return (!this._passengers.Any((Passenger p) => p._id == passengerId)) ? -1 : this._passengers.IndexOf(this._passengers.First((Passenger p) => p._id == passengerId));
		}
	}
}
