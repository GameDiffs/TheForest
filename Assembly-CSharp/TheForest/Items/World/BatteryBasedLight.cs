using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.World
{
	[AddComponentMenu("Items/World/Battery Based Light")]
	public class BatteryBasedLight : EntityBehaviour<IPlayerState>
	{
		public PlayerInventory _player;

		public Light _mainLight;

		public Color _torchBaseColor;

		public Color _torchBloodyColor;

		public float _batterieCostPerSecond = 0.2f;

		public float _delayBeforeLight = 0.5f;

		private void Awake()
		{
			this.SetColor(this._torchBaseColor);
		}

		private void OnEnable()
		{
			if (!BoltNetwork.isRunning || (BoltNetwork.isRunning && this.entity.isAttached && this.entity.isOwner))
			{
				base.StartCoroutine(this.DelayedLightOn());
			}
		}

		private void OnDisable()
		{
			this.SetEnabled(false);
		}

		private void Update()
		{
			if (!BoltNetwork.isRunning || (BoltNetwork.isRunning && this.entity.isAttached && this.entity.isOwner))
			{
				LocalPlayer.Stats.BatteryCharge -= this._batterieCostPerSecond * Time.deltaTime;
				if (LocalPlayer.Stats.BatteryCharge > 50f)
				{
					this.SetIntensity(0.45f);
				}
				else if (LocalPlayer.Stats.BatteryCharge < 20f)
				{
					if (LocalPlayer.Stats.BatteryCharge < 10f)
					{
						if (LocalPlayer.Stats.BatteryCharge < 5f)
						{
							if (LocalPlayer.Stats.BatteryCharge <= 0f)
							{
								this._player.StashLeftHand();
							}
							else
							{
								this.TorchLowerLightEvenMore();
							}
						}
						else
						{
							this.TorchLowerLightMore();
						}
					}
					else
					{
						this.TorchLowerLight();
					}
				}
				if (BoltNetwork.isRunning)
				{
					base.state.BatteryTorchIntensity = this._mainLight.intensity;
					base.state.BatteryTorchEnabled = this._mainLight.enabled;
					base.state.BatteryTorchColor = this._mainLight.color;
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedLightOn()
		{
			BatteryBasedLight.<DelayedLightOn>c__Iterator16E <DelayedLightOn>c__Iterator16E = new BatteryBasedLight.<DelayedLightOn>c__Iterator16E();
			<DelayedLightOn>c__Iterator16E.<>f__this = this;
			return <DelayedLightOn>c__Iterator16E;
		}

		private void GotBloody()
		{
			this.SetColor(this._torchBloodyColor);
		}

		private void GotClean()
		{
			this.SetColor(this._torchBaseColor);
		}

		private void TorchLowerLight()
		{
			this.SetIntensity(UnityEngine.Random.Range(0.4f, 0.3f));
		}

		private void TorchLowerLightMore()
		{
			this.SetIntensity(UnityEngine.Random.Range(0.35f, 0.2f));
		}

		private void TorchLowerLightEvenMore()
		{
			this.SetIntensity(UnityEngine.Random.Range(0.3f, 0.03f));
		}

		public void SetEnabled(bool enabled)
		{
			this._mainLight.enabled = enabled;
		}

		public void SetIntensity(float intensity)
		{
			this._mainLight.intensity = intensity;
		}

		public void SetColor(Color color)
		{
			this._mainLight.color = color;
		}
	}
}
