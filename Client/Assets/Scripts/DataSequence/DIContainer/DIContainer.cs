using System;
using System.Collections.Generic;

namespace GameEngine.DataSequence.DIContainer
{
    public enum Lifetime
    {
        Singleton,
        Transient,
    }

    public class Registration
    {
        public Func<object> Factory;
        public Lifetime Lifetime;
        public object Instance;
    }

    public static class DIContainer
    {
        private static Dictionary<Type, Registration> _registrations = new();

        public static void Register<TService>(Lifetime lifetime = Lifetime.Transient)
            where TService : new()
        {
            _registrations[typeof(TService)] = new Registration
            {
                Lifetime = lifetime,
                Instance = new TService()
            };
        }

        public static void Register<TService>(Func<object> factory, Lifetime lifetime = Lifetime.Transient)
            where TService : new()
        {
            _registrations[typeof(TService)] = new Registration
            {
                Lifetime = lifetime,
                Factory = factory,
                Instance = null
            };
        }

        public static void RegisterInstance<TService>(TService instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            _registrations[typeof(TService)] = new Registration
            {
                Lifetime = Lifetime.Singleton,
                Instance = instance!
            };
        }

        public static TService Resolve<TService>()
        {
            var t = typeof(TService);
            if (_registrations.TryGetValue(t, out var reg) == false)
                throw new InvalidOperationException($"[DI] Registration not found for service '{t.FullName}'.");

            if (reg.Lifetime == Lifetime.Singleton)
            {
                if (reg.Instance == null)
                    reg.Instance = reg.Factory();
                return (TService)reg.Instance;
            }

            return (TService)reg.Factory();
        }
    }
}