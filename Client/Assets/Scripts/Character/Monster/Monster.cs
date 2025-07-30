using GameEngine.DataSequence.StateMachine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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
            player = GameObject.FindObjectOfType<CharacterController>();
            shootPattern = GetComponent<FanShapeShooting>();
            sm = new(this.transform);

            var (stateList, defaultState) = GetStatesAndDefault();
            stateList.ForEach(state => sm.AddState(state));
            sm.ChangeState(defaultState);
            ability = GetComponent<UnitAbility>();
            ability.Health.OnValueChanged += x =>
            {
                if (x <= 0)
                    Destroy(gameObject);
            };
        }

        protected virtual (List<UnitState> states, Type defaultState) GetStatesAndDefault()
        {
            var states = new List<UnitState>();
            states.Add(new WalkState(GetDistance));
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

        private Vector2 GetDistance() => player.transform.position - transform.position;

        private void Shoot()
        {
            shootPattern.Shoot(player.transform.position);
        }
    }
}