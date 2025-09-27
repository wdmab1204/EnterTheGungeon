using GameEngine.Observable;
using UnityEngine;

namespace GameEngine
{
    public interface IUnitAbility
    {
        ReactiveProperty<int> MaxHealth { get; set; }
        ReactiveProperty<int> Health { get; set; }
    }

    public class UnitAbility : MonobehaviourExtension, IUnitAbility
    {
        public ReactiveProperty<int> MaxHealth { get; set; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> Health { get; set; } = new ReactiveProperty<int>();

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
