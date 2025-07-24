using GameEngine.DataSequence.StateMachine;
using UnityEngine;

namespace GameEngine
{
    public class Monster : UnityEngine.MonoBehaviour
    {
        private CharacterController player;
        private UnitStateMachine sm;
        private FanShapeShooting shootPattern;
        private UnitAbility ability;
        
        void Start()
        {
            player = GameObject.FindObjectOfType<CharacterController>();
            shootPattern = GetComponent<FanShapeShooting>();
            sm = new(this.transform);
            sm.AddState(new WalkState(GetDistance), new ShootState(Shoot));
            sm.ChangeState(typeof(WalkState));
            ability = GetComponent<UnitAbility>();
            ability.Health.OnValueChanged += x =>
            {
                if (x <= 0)
                    Destroy(gameObject);
            };
        }

        private void Update()
        {
            sm.Update();
        }

        private void FixedUpdate()
        {
            sm.FixedUpdate();
        }

        private Vector2 GetDistance() => player.transform.position - transform.position;

        private void Shoot()
        {
            shootPattern.Shoot(player.transform.position);
        }
    }
}