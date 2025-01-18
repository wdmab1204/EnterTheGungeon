using System;
using GameEngine.Characters.State;
using UnityEngine;

namespace DataSequence
{
    public class WalkState : UnitState
    {
        private Func<Vector2> getDistance;
        private float speed = 1;
        private float atkRange = 5;
        private Rigidbody2D rb;
        
        public WalkState(Func<Vector2> getDistance)
        {
            this.getDistance = getDistance;
        }

        public override void Enter()
        {
            if (rb == null)
                rb = transform.GetComponent<Rigidbody2D>();
        }

        public override void FixedUpdate(float time)
        {
            Vector2 distance = getDistance();
            if (atkRange >= distance.magnitude)
            {
                changeState(typeof(ShootState));
                return;
            }
               
            rb.MovePosition(rb.position + distance.normalized * speed * Time.fixedDeltaTime);
        }
    }
}