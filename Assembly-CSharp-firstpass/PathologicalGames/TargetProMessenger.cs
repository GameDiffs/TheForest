using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/TargetPro/Messenger")]
	public class TargetProMessenger : MonoBehaviour
	{
		public enum COMPONENTS
		{
			FireController,
			Projectile,
			Targetable
		}

		public enum MESSAGE_MODE
		{
			Send,
			Broadcast
		}

		public TargetProMessenger.COMPONENTS forComponent;

		public TargetProMessenger.MESSAGE_MODE messageMode;

		public GameObject otherTarget;

		public DEBUG_LEVELS debugLevel;

		public bool fireController_OnStart;

		public bool fireController_OnUpdate;

		public bool fireController_OnTargetUpdate;

		public bool fireController_OnIdleUpdate;

		public bool fireController_OnFire;

		public bool fireController_OnStop;

		public bool projectile_OnLaunched;

		public bool projectile_OnLaunchedUpdate;

		public bool projectile_OnDetonation;

		public bool targetable_OnHit;

		public bool targetable_OnDetected;

		public bool targetable_OnNotDetected;

		private void Awake()
		{
			this.RegisterFireController();
			this.RegisterProjectile();
			this.RegisterTargetable();
		}

		private void handleMsg(string msg)
		{
			GameObject gameObject;
			if (this.otherTarget == null)
			{
				gameObject = base.gameObject;
			}
			else
			{
				gameObject = this.otherTarget;
			}
			if (this.debugLevel > DEBUG_LEVELS.Off)
			{
				Debug.Log(string.Format("Sending message '{0}' to '{1}'", msg, gameObject));
			}
			if (this.messageMode == TargetProMessenger.MESSAGE_MODE.Send)
			{
				gameObject.SendMessage(msg, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				gameObject.BroadcastMessage(msg, SendMessageOptions.DontRequireReceiver);
			}
		}

		private void handleMsg<T>(string msg, T arg)
		{
			GameObject gameObject;
			if (this.otherTarget == null)
			{
				gameObject = base.gameObject;
			}
			else
			{
				gameObject = this.otherTarget;
			}
			if (this.debugLevel > DEBUG_LEVELS.Off)
			{
				Debug.Log(string.Format("Sending message '{0}' to '{1}' with argument {2}", msg, gameObject, arg));
			}
			if (this.messageMode == TargetProMessenger.MESSAGE_MODE.Send)
			{
				gameObject.SendMessage(msg, arg, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				gameObject.BroadcastMessage(msg, arg, SendMessageOptions.DontRequireReceiver);
			}
		}

		private void RegisterFireController()
		{
			FireController component = base.GetComponent<FireController>();
			if (component == null)
			{
				return;
			}
			component.AddOnStartDelegate(new FireController.OnStartDelegate(this.OnStartDelegate));
			component.AddOnUpdateDelegate(new FireController.OnUpdateDelegate(this.OnUpdateDelegate));
			component.AddOnTargetUpdateDelegate(new FireController.OnTargetUpdateDelegate(this.OnTargetUpdateDelegate));
			component.AddOnIdleUpdateDelegate(new FireController.OnIdleUpdateDelegate(this.OnIdleUpdateDelegate));
			component.AddOnFireDelegate(new FireController.OnFireDelegate(this.OnFireDelegate));
			component.AddOnStopDelegate(new FireController.OnStopDelegate(this.OnStopDelegate));
		}

		private void OnStartDelegate()
		{
			if (!this.fireController_OnStart)
			{
				return;
			}
			this.handleMsg("FireController_OnStart");
		}

		private void OnUpdateDelegate()
		{
			if (!this.fireController_OnUpdate)
			{
				return;
			}
			this.handleMsg("FireController_OnUpdate");
		}

		private void OnTargetUpdateDelegate(List<Target> targets)
		{
			if (!this.fireController_OnTargetUpdate)
			{
				return;
			}
			this.handleMsg<List<Target>>("FireController_OnTargetUpdate", targets);
		}

		private void OnIdleUpdateDelegate()
		{
			if (!this.fireController_OnIdleUpdate)
			{
				return;
			}
			this.handleMsg("FireController_OnIdleUpdate");
		}

		private void OnFireDelegate(List<Target> targets)
		{
			if (!this.fireController_OnFire)
			{
				return;
			}
			this.handleMsg<List<Target>>("FireController_OnFire", targets);
		}

		private void OnStopDelegate()
		{
			if (!this.fireController_OnStop)
			{
				return;
			}
			this.handleMsg("Projectile_OnStop");
		}

		private void RegisterProjectile()
		{
			Projectile component = base.GetComponent<Projectile>();
			if (component == null)
			{
				return;
			}
			component.AddOnLaunchedDelegate(new Projectile.OnLaunched(this.OnLauchedDelegate));
			component.AddOnLaunchedUpdateDelegate(new Projectile.OnLaunchedUpdate(this.LaunchedUpdateDelegate));
			component.AddOnDetonationDelegate(new Projectile.OnDetonation(this.OnDetonationDelegate));
		}

		private void OnLauchedDelegate()
		{
			if (!this.projectile_OnLaunched)
			{
				return;
			}
			this.handleMsg("Projectile_OnLauched");
		}

		private void LaunchedUpdateDelegate()
		{
			if (!this.projectile_OnLaunchedUpdate)
			{
				return;
			}
			this.handleMsg("Projectile_LaunchedUpdate");
		}

		private void OnDetonationDelegate(List<Target> targets)
		{
			if (!this.projectile_OnDetonation)
			{
				return;
			}
			this.handleMsg<List<Target>>("Projectile_OnDetonation", targets);
		}

		private void RegisterTargetable()
		{
			Targetable component = base.GetComponent<Targetable>();
			if (component == null)
			{
				return;
			}
			component.AddOnHitDelegate(new Targetable.OnHitDelegate(this.OnHitDelegate));
			component.AddOnDetectedDelegate(new Targetable.OnDetectedDelegate(this.OnDetectedDelegate));
			component.AddOnNotDetectedDelegate(new Targetable.OnNotDetectedDelegate(this.OnNotDetectedDelegate));
		}

		private void OnHitDelegate(HitEffectList effects, Target target)
		{
			if (!this.targetable_OnHit)
			{
				return;
			}
			MessageData_TargetableOnHit arg = new MessageData_TargetableOnHit(effects, target);
			this.handleMsg<MessageData_TargetableOnHit>("Targetable_OnHit", arg);
		}

		private void OnDetectedDelegate(TargetTracker source)
		{
			if (!this.targetable_OnDetected)
			{
				return;
			}
			this.handleMsg<TargetTracker>("Targetable_OnDetected", source);
		}

		private void OnNotDetectedDelegate(TargetTracker source)
		{
			if (!this.targetable_OnNotDetected)
			{
				return;
			}
			this.handleMsg<TargetTracker>("Targetable_OnNotDetected", source);
		}
	}
}
