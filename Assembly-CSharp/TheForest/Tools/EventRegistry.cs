using System;
using System.Collections.Generic;
using UniLinq;

namespace TheForest.Tools
{
	public class EventRegistry
	{
		private class EventSubscription
		{
			public int _publishingEventIndex;

			public IList<EventRegistry.SubscriberCallback> _callbacks = new List<EventRegistry.SubscriberCallback>();
		}

		public delegate void SubscriberCallback(object eventParameter);

		private IDictionary<object, EventRegistry.EventSubscription> _eventSubscriptions = new Dictionary<object, EventRegistry.EventSubscription>();

		public static EventRegistry Player = new EventRegistry();

		public static EventRegistry Enemy = new EventRegistry();

		public static EventRegistry Animal = new EventRegistry();

		public void Subscribe(object eventType, EventRegistry.SubscriberCallback newSubscriber)
		{
			EventRegistry.EventSubscription eventSubscription;
			if (this._eventSubscriptions.ContainsKey(eventType))
			{
				eventSubscription = this._eventSubscriptions[eventType];
			}
			else
			{
				eventSubscription = new EventRegistry.EventSubscription();
				this._eventSubscriptions[eventType] = eventSubscription;
			}
			eventSubscription._callbacks.Add(newSubscriber);
		}

		public void Unsubscribe(object eventType, EventRegistry.SubscriberCallback unsubscriber)
		{
			if (this._eventSubscriptions.ContainsKey(eventType))
			{
				EventRegistry.EventSubscription eventSubscription = this._eventSubscriptions[eventType];
				if (eventSubscription._publishingEventIndex > -1)
				{
					int num = eventSubscription._callbacks.IndexOf(unsubscriber);
					if (num < eventSubscription._publishingEventIndex)
					{
						eventSubscription._publishingEventIndex--;
					}
				}
				eventSubscription._callbacks.Remove(unsubscriber);
			}
		}

		public void Publish(object eventType, object eventParameter)
		{
			if (this._eventSubscriptions.ContainsKey(eventType))
			{
				EventRegistry.EventSubscription eventSubscription = this._eventSubscriptions[eventType];
				eventSubscription._publishingEventIndex = eventSubscription._callbacks.Count - 1;
				while (eventSubscription._publishingEventIndex >= 0)
				{
					eventSubscription._callbacks[eventSubscription._publishingEventIndex](eventParameter);
					eventSubscription._publishingEventIndex--;
				}
				eventSubscription._publishingEventIndex = -1;
			}
		}

		public static void Clear()
		{
			EventRegistry.Player._eventSubscriptions.ForEach(delegate(KeyValuePair<object, EventRegistry.EventSubscription> es)
			{
				es.Value._callbacks.Clear();
			});
			EventRegistry.Player._eventSubscriptions.Clear();
			EventRegistry.Enemy._eventSubscriptions.ForEach(delegate(KeyValuePair<object, EventRegistry.EventSubscription> es)
			{
				es.Value._callbacks.Clear();
			});
			EventRegistry.Enemy._eventSubscriptions.Clear();
			EventRegistry.Animal._eventSubscriptions.ForEach(delegate(KeyValuePair<object, EventRegistry.EventSubscription> es)
			{
				es.Value._callbacks.Clear();
			});
			EventRegistry.Animal._eventSubscriptions.Clear();
		}
	}
}
