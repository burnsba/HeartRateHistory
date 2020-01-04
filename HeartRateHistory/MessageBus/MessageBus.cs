using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace HeartRateHistory.MessageBus
{
    /// <summary>
    /// Publish/subscribe message bus. Allows event notification across view models without attaching event handlers.
    /// This should only save weak references to objects.
    /// </summary>
    public static class MessageBus
    {
        /// <summary>
        /// Collection of known subscriptions.
        /// </summary>
        private static List<SubscriptionContainerInfo> _subscriptions = new List<SubscriptionContainerInfo>();

        /// <summary>
        /// Creates a new subscription. If a subscription with the same parameters already exists, nothing happens.
        /// </summary>
        /// <typeparam name="TSubscribeTo">Type of object being subscribed to, that will transmit event.</typeparam>
        /// <typeparam name="TSubscriber">Type of object receiving notifications.</typeparam>
        /// <param name="subscribeToEvent">Name of the event to subscribe to.</param>
        /// <param name="subscriber">Object receiving notifications.</param>
        /// <param name="eventHandlerMethodName">Name of method that will be invoked via reflection to handle notification.</param>
        public static void Subscribe<TSubscribeTo, TSubscriber>(string subscribeToEvent, TSubscriber subscriber, string eventHandlerMethodName)
        {
            SubscriptionContainerInfo newSubscriptionContainer = null;
            SubscriptionEventInfo newEventInfo = null;
            SubscriberInfo newSubscriber = null;
            var subscribeToContainerType = typeof(TSubscribeTo);
            var subscriberType = typeof(TSubscriber);
            var currentSubscriptions = _subscriptions.Where(x => x.SubscribeToContainerType == subscribeToContainerType);
            foreach (var subscriptionContainer in currentSubscriptions)
            {
                var eventInfos = subscriptionContainer.Events.Where(x => x.EventName == subscribeToEvent);
                foreach (var eventInfo in eventInfos)
                {
                    var existingSubscriber = eventInfo.Subscribers.Where(x => x.SubscriberType == subscriberType && x.EventHandlerMethodName == eventHandlerMethodName);
                    if (!object.ReferenceEquals(null, existingSubscriber))
                    {
                        return;
                    }

                    newSubscriber = new SubscriberInfo(eventInfo, subscriber, subscriberType, eventHandlerMethodName);
                    eventInfo.Subscribers.Add(newSubscriber);
                    return;
                }

                // else, not found in Events.
                newEventInfo = new SubscriptionEventInfo(subscriptionContainer, subscribeToEvent);
                newSubscriber = new SubscriberInfo(newEventInfo, subscriber, subscriberType, eventHandlerMethodName);
                newEventInfo.Subscribers.Add(newSubscriber);
                subscriptionContainer.Events.Add(newEventInfo);
                return;
            }

            // else, not found in current subscriptions
            newSubscriptionContainer = new SubscriptionContainerInfo(subscribeToContainerType);
            newEventInfo = new SubscriptionEventInfo(newSubscriptionContainer, subscribeToEvent);
            newSubscriber = new SubscriberInfo(newEventInfo, subscriber, subscriberType, eventHandlerMethodName);
            newEventInfo.Subscribers.Add(newSubscriber);
            newSubscriptionContainer.Events.Add(newEventInfo);
            _subscriptions.Add(newSubscriptionContainer);
        }

        /// <summary>
        /// Creates a new subscription. If a subscription with the same parameters already exists, nothing happens.
        /// </summary>
        /// <typeparam name="TSubscribeTo">Type of object being subscribed to, that will transmit event.</typeparam>
        /// <typeparam name="TSubscriber">Type of object receiving notifications.</typeparam>
        /// <param name="subscribeToEvent">Name of the event to subscribe to.</param>
        /// <param name="subscriber">Object receiving notifications.</param>
        /// <param name="eventHandler">Method that will be invoked to handle notification.</param>
        public static void Subscribe<TSubscribeTo, TSubscriber>(string subscribeToEvent, TSubscriber subscriber, EventHandler eventHandler)
        {
            SubscriptionContainerInfo newSubscriptionContainer = null;
            SubscriptionEventInfo newEventInfo = null;
            SubscriberInfo newSubscriber = null;
            var subscribeToContainerType = typeof(TSubscribeTo);
            var subscriberType = typeof(TSubscriber);
            var currentSubscriptions = _subscriptions.Where(x => x.SubscribeToContainerType == subscribeToContainerType);
            foreach (var subscriptionContainer in currentSubscriptions)
            {
                var eventInfos = subscriptionContainer.Events.Where(x => x.EventName == subscribeToEvent);
                foreach (var eventInfo in eventInfos)
                {
                    var existingSubscriber = eventInfo
                        .Subscribers
                        .Where(x => x.SubscriberType == subscriberType && x.Handler == eventHandler)
                        .FirstOrDefault();

                    if (!object.ReferenceEquals(null, existingSubscriber))
                    {
                        return;
                    }

                    newSubscriber = new SubscriberInfo(eventInfo, subscriber, subscriberType, eventHandler);
                    eventInfo.Subscribers.Add(newSubscriber);
                    return;
                }

                // else, not found in Events.
                newEventInfo = new SubscriptionEventInfo(subscriptionContainer, subscribeToEvent);
                newSubscriber = new SubscriberInfo(newEventInfo, subscriber, subscriberType, eventHandler);
                newEventInfo.Subscribers.Add(newSubscriber);
                subscriptionContainer.Events.Add(newEventInfo);
                return;
            }

            // else, not found in current subscriptions
            newSubscriptionContainer = new SubscriptionContainerInfo(subscribeToContainerType);
            newEventInfo = new SubscriptionEventInfo(newSubscriptionContainer, subscribeToEvent);
            newSubscriber = new SubscriberInfo(newEventInfo, subscriber, subscriberType, eventHandler);
            newEventInfo.Subscribers.Add(newSubscriber);
            newSubscriptionContainer.Events.Add(newEventInfo);
            _subscriptions.Add(newSubscriptionContainer);
        }

        /// <summary>
        /// Notify listeners of event.
        /// </summary>
        /// <param name="eventSource">Object sending notification.</param>
        /// <param name="notificationEvent">Name of the event sending notification.</param>
        /// <param name="args">Notification arguments.</param>
        public static void Notify(object eventSource, string notificationEvent, EventArgs args)
        {
            var toBeNotified = _subscriptions.Where(x => x.SubscribeToContainerType.IsAssignableFrom(eventSource.GetType()));

            var exceptions = new List<Exception>();
            bool anyExceptions = false;

            foreach (var subscriptionInfo in toBeNotified)
            {
                try
                {
                    subscriptionInfo.Notify(eventSource, notificationEvent, args);
                }
                catch (AggregateException aex)
                {
                    anyExceptions = true;
                    exceptions.Add(aex);
                    continue;
                }
                catch (Exception ex)
                {
                    anyExceptions = true;
                    exceptions.Add(ex);
                    continue;
                }
            }

            if (anyExceptions)
            {
                throw new AggregateException(exceptions);
            }
        }

        /// <summary>
        /// Used to determine if a weak reference is still valid by casting to type.
        /// </summary>
        /// <param name="obj">Reference to attempt cast.</param>
        /// <param name="t">Type to cast to.</param>
        /// <returns>True if cast is non-null, false otherwise.</returns>
        private static bool CastIsValid(object obj, Type t)
        {
            try
            {
                var castedObject = Expression.Lambda(Expression.Convert(Expression.Constant(obj), t)).Compile().DynamicInvoke();
                return !object.ReferenceEquals(null, castedObject);
            }
            catch (TargetInvocationException)
            {
                return false;
            }
        }

        private static void RemoveSubscriptionContainer(SubscriptionContainerInfo item)
        {
            _subscriptions.Remove(item);

            item = null;
        }

        /// <summary>
        /// Top level subscription information.
        /// </summary>
        private class SubscriptionContainerInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SubscriptionContainerInfo"/> class.
            /// </summary>
            /// <param name="subscribeToContainerType">Type of object being subscribed to, that will transmit event.</param>
            public SubscriptionContainerInfo(Type subscribeToContainerType)
            {
                Events = new List<SubscriptionEventInfo>();
                SubscribeToContainerType = subscribeToContainerType;
            }

            /// <summary>
            /// Gets the type that will send notification events.
            /// </summary>
            public Type SubscribeToContainerType { get; private set; }

            /// <summary>
            /// Gets the list of events that might send notifications.
            /// </summary>
            public List<SubscriptionEventInfo> Events { get; private set; }

            /// <summary>
            /// Notify listeners of event.
            /// </summary>
            /// <param name="eventSource">Object sending notification.</param>
            /// <param name="notificationEvent">Name of the event sending notification.</param>
            /// <param name="args">Notification arguments.</param>
            public void Notify(object eventSource, string notificationEvent, EventArgs args)
            {
                var toBeNotified = Events.Where(x => x.EventName == notificationEvent);

                var exceptions = new List<Exception>();
                bool anyExceptions = false;

                foreach (var subscriptionInfo in toBeNotified)
                {
                    try
                    {
                        subscriptionInfo.Notify(eventSource, args);
                    }
                    catch (AggregateException aex)
                    {
                        anyExceptions = true;
                        exceptions.Add(aex);
                        continue;
                    }
                    catch (Exception ex)
                    {
                        anyExceptions = true;
                        exceptions.Add(ex);
                        continue;
                    }
                }

                if (anyExceptions)
                {
                    throw new AggregateException(exceptions);
                }
            }

            /// <summary>
            /// Removes child from list of events.
            /// </summary>
            /// <param name="item">Child to remove.</param>
            public void InvalidateChildReference(SubscriptionEventInfo item)
            {
                Events.Remove(item);

                item = null;

                if (Events.Count == 0)
                {
                    InvalidateReference();
                }
            }

            /// <summary>
            /// Notification to remove this from parent collection.
            /// </summary>
            public void InvalidateReference()
            {
                RemoveSubscriptionContainer(this);
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return SubscribeToContainerType.FullName;
            }
        }

        /// <summary>
        /// Information about event on an object.
        /// </summary>
        private class SubscriptionEventInfo
        {
            private SubscriptionContainerInfo _parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="SubscriptionEventInfo"/> class.
            /// </summary>
            /// <param name="parent">Parent container.</param>
            /// <param name="eventName">Name of the event sending notification.</param>
            public SubscriptionEventInfo(SubscriptionContainerInfo parent, string eventName)
            {
                Subscribers = new List<SubscriberInfo>();

                EventName = eventName;
                _parent = parent;
            }

            /// <summary>
            /// Gets name of the event sending notification.
            /// </summary>
            public string EventName { get; private set; }

            /// <summary>
            /// Gets the list of subscribers for this event.
            /// </summary>
            public List<SubscriberInfo> Subscribers { get; private set; }

            /// <summary>
            /// Notify listeners of event.
            /// </summary>
            /// <param name="eventSource">Object sending notification.</param>
            /// <param name="args">Notification arguments.</param>
            public void Notify(object eventSource, EventArgs args)
            {
                var exceptions = new List<Exception>();
                bool anyExceptions = false;

                foreach (var subscriber in Subscribers)
                {
                    try
                    {
                        subscriber.Notify(eventSource, args);
                    }
                    catch (AggregateException aex)
                    {
                        anyExceptions = true;
                        exceptions.Add(aex);
                        continue;
                    }
                    catch (Exception ex)
                    {
                        anyExceptions = true;
                        exceptions.Add(ex);
                        continue;
                    }
                }

                if (anyExceptions)
                {
                    throw new AggregateException(exceptions);
                }
            }

            /// <summary>
            /// Removes child from list of subscribers.
            /// </summary>
            /// <param name="item">Child to remove.</param>
            public void InvalidateChildReference(SubscriberInfo item)
            {
                Subscribers.Remove(item);

                item = null;

                if (Subscribers.Count == 0)
                {
                    InvalidateReference();
                }
            }

            /// <summary>
            /// Notification to remove this from parent collection.
            /// </summary>
            public void InvalidateReference()
            {
                _parent.InvalidateChildReference(this);
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return $"{_parent.ToString()}.{EventName}";
            }
        }

        /// <summary>
        /// Information about a subscriber for a particular event on a particular type of object.
        /// </summary>
        private class SubscriberInfo
        {
            private WeakReference _subscriberReference;
            private SubscriptionEventInfo _parent;
            private MethodInfo _methodInfo = null;

            /// <summary>
            /// Initializes a new instance of the <see cref="SubscriberInfo"/> class.
            /// </summary>
            /// <param name="parent">Parent container.</param>
            /// <param name="subscriber">Object receiving notifications.</param>
            /// <param name="subscriberType">Type of object receiving notifications.</param>
            /// <param name="eventHandlerMethodName">Name of method that will be invoked via reflection to handle notification.</param>
            public SubscriberInfo(SubscriptionEventInfo parent, object subscriber, Type subscriberType, string eventHandlerMethodName)
            {
                if (object.ReferenceEquals(null, subscriber))
                {
                    throw new ArgumentNullException(nameof(subscriber));
                }

                _subscriberReference = new WeakReference(subscriber);
                SubscriberType = subscriberType;
                EventHandlerMethodName = eventHandlerMethodName;
                Handler = null;
                _parent = parent;

                _methodInfo = SubscriberType.GetMethod(EventHandlerMethodName);

                if (object.ReferenceEquals(null, _methodInfo))
                {
                    throw new ArgumentException($"Could not find {eventHandlerMethodName} on type {subscriberType.FullName}");
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SubscriberInfo"/> class.
            /// </summary>
            /// <param name="parent">Parent container.</param>
            /// <param name="subscriber">Object receiving notifications.</param>
            /// <param name="subscriberType">Type of object receiving notifications.</param>
            /// <param name="eventHandler">Method that will be invoked to handle notification.</param>
            public SubscriberInfo(SubscriptionEventInfo parent, object subscriber, Type subscriberType, EventHandler eventHandler)
            {
                if (object.ReferenceEquals(null, subscriber))
                {
                    throw new ArgumentNullException(nameof(subscriber));
                }

                _subscriberReference = new WeakReference(subscriber);
                SubscriberType = subscriberType;
                EventHandlerMethodName = null;
                _parent = parent;

                Handler = eventHandler;
            }

            /// <summary>
            /// Gets type of object receiving notifications.
            /// </summary>
            public Type SubscriberType { get; private set; }

            /// <summary>
            /// Gets name of method that will be invoked via reflection to handle notification.
            /// </summary>
            public string EventHandlerMethodName { get; private set; }

            /// <summary>
            /// Gets method that will be invoked to handle notification.
            /// </summary>
            public EventHandler Handler { get; private set; }

            /// <summary>
            /// Notify listeners of event.
            /// </summary>
            /// <param name="eventSource">Object sending notification.</param>
            /// <param name="args">Notification arguments.</param>
            public void Notify(object eventSource, EventArgs args)
            {
                if (!object.ReferenceEquals(null, Handler))
                {
                    Handler?.Invoke(eventSource, args);
                }
                else if (!object.ReferenceEquals(null, _methodInfo))
                {
                    var currentTarget = _subscriberReference.Target;
                    if (CastIsValid(currentTarget, SubscriberType))
                    {
                        _methodInfo.Invoke(currentTarget, new object[] { eventSource, args });
                    }
                    else
                    {
                        InvalidateReference();
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Either {nameof(Handler)} or {nameof(_methodInfo)} must be set");
                }
            }

            /// <summary>
            /// Notification to remove this from parent collection.
            /// </summary>
            public void InvalidateReference()
            {
                _parent.InvalidateChildReference(this);
            }

            /// <inheritdoc />
            public override string ToString()
            {
                if (!string.IsNullOrEmpty(EventHandlerMethodName))
                {
                    return $"{_parent.ToString()}.{SubscriberType.FullName}.{EventHandlerMethodName}";
                }

                return $"{_parent.ToString()}.{SubscriberType.FullName}.{Handler.Method.Name}";
            }
        }
    }
}
