using System;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/World/Rain Collector Held")]
	public class RainCollectorHeld : MonoBehaviour
	{
		public InventoryItemView _source;

		[SerializeThis]
		private float _lastUpdateTime = -1f;

		private void OnEnable()
		{
			this._lastUpdateTime = Scene.Clock.ElapsedGameTime;
		}

		private void Update()
		{
			this.CheckForWater();
		}

		private void CheckForWater()
		{
			float elapsedGameTime = Scene.Clock.ElapsedGameTime;
			if (Scene.WeatherSystem.Raining && this._lastUpdateTime + 0.01f < elapsedGameTime)
			{
				this._source.ActiveBonus = WeaponStatUpgrade.Types.CleanWater;
			}
			if (this._source.ActiveBonus == WeaponStatUpgrade.Types.CleanWater)
			{
				this._lastUpdateTime = elapsedGameTime;
			}
		}
	}
}
