using GameEngine.Observable;
using UnityEngine;

namespace GameEngine
{
    public class UnitAbility : MonobehaviourExtension
    {
        public ReactiveProperty<int> MaxHealth = new();
        public ReactiveProperty<int> Health = new();

        [SerializeField] private int maxHelath;
        [SerializeField] private int health;

        protected virtual void Awake()
        {
            Health.Value = Mathf.Min(health, maxHelath);
            Health.OnValueChanged += x =>
            {
                health = x;
            };

            MaxHealth.Value = maxHelath;
            MaxHealth.OnValueChanged += x =>
            {
                maxHelath = x;
            };
        }
    }
}
