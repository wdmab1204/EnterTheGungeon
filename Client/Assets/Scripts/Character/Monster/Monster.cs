using System;
using DataSequence;
using GameEngine.Characters.State;
using UnityEngine;

namespace GameEngine
{
    public class Monster : UnityEngine.MonoBehaviour
    {
        private CharacterController player;
        private UnitStateMachine sm;
        private FanShapeShooting shootPattern;
        
        void Start()
        {
            player = GameObject.FindObjectOfType<CharacterController>();
            shootPattern = GetComponent<FanShapeShooting>();
            sm = new(this.transform);
            sm.AddState(new WalkState(GetDirection), new ShootState(Shoot));
            sm.ChangeState(typeof(WalkState));
        }

        private void Update()
        {
            sm.Update();
        }

        private Vector2 GetDirection() => player.transform.position - transform.position;

        private void Shoot()
        {
            shootPattern.Shoot(player.transform.position);
        }
    }
}