using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	public abstract class NotifyOnEvent : MonoBehaviour
	{
		private interface INotify
		{
		}

		private class Notify : NotifyOnEvent.INotify
		{
			public Action<GameObject> action;
		}

		private class NotifyWithArg : NotifyOnEvent.INotify
		{
			public Action<GameObject, object> action;

			public object arg;
		}

		public static bool Disable;

		private IList<NotifyOnEvent.INotify> m_actions = new List<NotifyOnEvent.INotify>();

		protected void OnEvent()
		{
			if (NotifyOnEvent.Disable)
			{
				return;
			}
			int count = this.m_actions.Count;
			for (int i = 0; i < count; i++)
			{
				NotifyOnEvent.INotify notify = this.m_actions[i];
				if (notify is NotifyOnEvent.Notify)
				{
					NotifyOnEvent.Notify notify2 = notify as NotifyOnEvent.Notify;
					notify2.action(base.gameObject);
				}
				else if (notify is NotifyOnEvent.NotifyWithArg)
				{
					NotifyOnEvent.NotifyWithArg notifyWithArg = notify as NotifyOnEvent.NotifyWithArg;
					notifyWithArg.action(base.gameObject, notifyWithArg.arg);
				}
			}
		}

		public void AddAction(Action<GameObject, object> action, object arg)
		{
			NotifyOnEvent.NotifyWithArg notifyWithArg = new NotifyOnEvent.NotifyWithArg();
			notifyWithArg.action = action;
			notifyWithArg.arg = arg;
			this.m_actions.Add(notifyWithArg);
		}

		public void AddAction(Action<GameObject> action)
		{
			NotifyOnEvent.Notify notify = new NotifyOnEvent.Notify();
			notify.action = action;
			this.m_actions.Add(notify);
		}
	}
}
