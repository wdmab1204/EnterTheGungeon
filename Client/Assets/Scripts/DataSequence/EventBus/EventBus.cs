using System;
using System.Collections.Generic;

namespace GameEngine.DataSequence.EventBus
{
    public interface ISubscription
    {

    }

    public class Subscription<T> : ISubscription
    {
        Action<T> eventList;

        public void Subscribe(Action<T> action)
        {
            eventList += action;
        }

        public void UnSubscribe(Action<T> action)
        {
            eventList -= action;
        }
         
        public void Notify(T data)
        {
            eventList?.Invoke(data);
        }
    }

    public static class EventBus
    {
        private static readonly Dictionary<(string, Type), ISubscription> subscriptionDict = new();

        public static void Subscribe<T>(string eventName, Action<T> callback)
        {
            var key = (eventName, typeof(T));

            if (!subscriptionDict.TryGetValue(key, out var sub))
            {
                sub = new Subscription<T>();
                subscriptionDict[key] = sub;
            }

            ((Subscription<T>)sub).Subscribe(callback);
        }

        public static void UnSubScribe<T>(string eventName, Action<T> callback)
        {
            var key = (eventName, typeof(T));

            if (!subscriptionDict.TryGetValue(key, out var sub))
            {
                sub = new Subscription<T>();
                subscriptionDict[key] = sub;
            }

            ((Subscription<T>)sub).UnSubscribe(callback);
        }

        public static void Publish<T>(string eventName, T data)
        {
            var key = (eventName, typeof(T));

            if (subscriptionDict.TryGetValue(key, out var sub))
            {
                ((Subscription<T>)sub).Notify(data);
            }
        }
    }
}