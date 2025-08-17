using GameEngine.DataSequence.StateMachine;
using GameEngine.Navigation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    public class Monster : UnityEngine.MonoBehaviour
    {
        protected CharacterController player;
        private UnitStateMachine sm;
        private FanShapeShooting shootPattern;
        private UnitAbility ability;
        
        void Start()
        {
            player = GameData.Player;
            shootPattern = GetComponent<FanShapeShooting>();
            sm = new(this.transform);

            var (stateList, defaultState) = GetStatesAndDefault();
            stateList.ForEach(state => sm.AddState(state));
            sm.ChangeState(defaultState);
            ability = GetComponent<UnitAbility>();
            ability.Health.OnValueChanged += x =>
            {
                if (x <= 0)
                    Death();
            };
        }

        protected virtual (List<UnitState> states, Type defaultState) GetStatesAndDefault()
        {
            var states = new List<UnitState>();
            states.Add(new WalkState(PathFindManager.GetPath, player.transform, 1f, 5f));
            states.Add(new ShootState(Shoot));
            return (states, typeof(WalkState));
        }

        private void Update()
        {
            sm.Update();
        }

        private void FixedUpdate()
        {
            sm.FixedUpdate();
        }

        protected Vector2 GetDistance() => player.transform.position - transform.position;

        private void Shoot()
        {
            shootPattern.Shoot(player.transform.position);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                UnitAbility ability = collision.gameObject.GetComponent<UnitAbility>();
                ability.Health.Value -= 1;
            }
        }

        private void Death()
        {
            //something do

            Destroy(this.gameObject);
        }
    }
}