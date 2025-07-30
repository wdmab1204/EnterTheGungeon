using System;
using UnityEngine;

namespace GameEngine.DataSequence.StateMachine
{
    public class WalkState : UnitState
    {
        private Func<Vector2> getDistance;
        private float speed = 1;
        private float atkRange = 5;
        private Rigidbody2D rb;
        
        public WalkState(Func<Vector2> getDistance, float speed, float atkRange)
        {
            this.getDistance = getDistance;
            this.speed = speed;
            this.atkRange = atkRange;
        }

        public override void Enter()
        {
            if (rb == null)
                rb = transform.GetComponent<Rigidbody2D>();
        }

        public override void FixedUpdate(float time)
        {
            Vector2 dir = getDistance();
            Vector2 dirNormalized = dir.normalized;
            if (atkRange >= dir.magnitude)
            {
                changeState(typeof(ShootState));
                return;
            }

            Vector2 targetSpeed = new Vector2(dirNormalized.x, dirNormalized.y) * speed;

            targetSpeed = Vector2.Lerp(rb.velocity, targetSpeed, 1);

            float accelRate = (targetSpeed.magnitude > 0.01f) ? 10 : 15;

            Vector2 speedDif = targetSpeed - rb.velocity;
            Vector2 movement = speedDif * accelRate;

            rb.AddForce(movement, ForceMode2D.Force);
        }
    }
}