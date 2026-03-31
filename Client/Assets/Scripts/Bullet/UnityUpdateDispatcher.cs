using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityUpdateDispatcher : MonoBehaviour
{
    private List<Action<float>> subscribers = new();

    public void Register(Action<float> subscriber)
    {
        subscribers.Add(subscriber);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        foreach (var s in subscribers)
            s(dt);
    }

    public void Unregister(Action<float> subscriber)
    {
        subscribers.Remove(subscriber);
    }
}
