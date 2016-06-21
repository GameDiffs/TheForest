using Bolt;
using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/World/Rain Collector")]
	public class RainCollector : EntityBehaviour<IWaterSourceState>
	{
		public WaterSource _source;

		public GameObject _waterGO;

		public Transform _waterZeroTr;

		public Transform _waterFullTr;

		public float _waterPerSecond = 0.01f;

		public float _waterDryOutPerDay;

		[SerializeThis]
		private float _lastUpdateTime = -1f;

		private float _prevFill = -1f;

		private int _wsToken = -1;

		private float _nextWaterRefresh = -1f;

		private void Start()
		{
			if (this._lastUpdateTime == -1f)
			{
				this._lastUpdateTime = Scene.Clock.ElapsedGameTime;
			}
			if (BoltNetwork.isServer || !BoltNetwork.isRunning)
			{
				this.WSRegister();
			}
			base.enabled = false;
		}

		private void Update()
		{
			if (Scene.WeatherSystem.Raining)
			{
				this._source.AddWater(this._waterPerSecond * Time.deltaTime);
			}
			else
			{
				base.enabled = false;
			}
		}

		private void OnDestroy()
		{
			this.WSUnregister();
		}

		private void RainCheck()
		{
			if (this._nextWaterRefresh < Time.time)
			{
				this._nextWaterRefresh = Time.time + 60f;
				this.CheckForWater();
			}
			base.enabled = Scene.WeatherSystem.Raining;
		}

		private void CheckForWater()
		{
			if (!BoltNetwork.isClient)
			{
				float elapsedGameTime = Scene.Clock.ElapsedGameTime;
				this._source.RemoveWater((elapsedGameTime - this._lastUpdateTime) * this._waterDryOutPerDay);
				this._lastUpdateTime = elapsedGameTime;
			}
			else
			{
				this.UpdateWater();
			}
			if (BoltNetwork.isServer && this.entity && this.entity.isAttached && this.entity.isOwner && base.state.toggled != this._waterGO.activeSelf)
			{
				base.state.toggled = this._waterGO.activeSelf;
			}
		}

		private void UpdateWater()
		{
			bool flag = this._source.AmountReal > 0f;
			if (this._waterGO.activeSelf != flag)
			{
				this._waterGO.SetActive(flag);
			}
			if (flag)
			{
				float num = this._source.AmountReal / this._source._maxAmount;
				if (!Mathf.Approximately(num, this._prevFill))
				{
					this._waterGO.transform.position = Vector3.Lerp(this._waterZeroTr.position, this._waterFullTr.position, num);
					this._waterGO.transform.localScale = Vector3.Lerp(this._waterZeroTr.localScale, this._waterFullTr.localScale, num);
					this._prevFill = num;
				}
			}
		}

		private void WSRegister()
		{
			if (this._wsToken == -1)
			{
				this._wsToken = WorkScheduler.Register(new WorkScheduler.Task(this.RainCheck), base.transform.position, false);
			}
		}

		private void WSUnregister()
		{
			if (this._wsToken != -1)
			{
				WorkScheduler.Unregister(new WorkScheduler.Task(this.RainCheck), this._wsToken);
				this._wsToken = -1;
			}
		}

		public override void Attached()
		{
			base.state.AddCallback("toggled", delegate
			{
				this._waterGO.SetActive(base.state.toggled);
			});
		}
	}
}
