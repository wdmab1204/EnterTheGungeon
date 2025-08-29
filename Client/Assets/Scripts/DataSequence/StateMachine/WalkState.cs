using Cysharp.Threading.Tasks;
using GameEngine.Navigation;
using System;
using UnityEngine;

namespace GameEngine.DataSequence.StateMachine
{
    public class WalkState : UnitState
    {
        private float speed = 1;
        private float atkRange = 5;
        private Rigidbody2D rb;
        private Func<Vector3, Vector3, UniTask<PathResult>> PathRequest;
        private Vector3[] path;
        private float curTime = 0, interval = .3f;
        private int targetIndex = 0;
        private bool isRequestingPath;
        private Transform player;

        public WalkState(Func<Vector3, Vector3, UniTask<PathResult>> PathRequest, Transform player, float speed, float atkRange)
        {
            this.PathRequest = PathRequest;
            this.player = player;
            this.speed = speed;
            this.atkRange = atkRange;
        }

        public override void Enter()
        {
            if (rb == null)
                rb = transform.GetComponent<Rigidbody2D>();
        }

        public override void Exit()
        {
            rb.velocity = Vector2.zero;
        }

        public override void TickUpdate(float time)
        {
            if (curTime < interval)
            {
                curTime += time;
            }
            else
            {
                if (!isRequestingPath)
                {
                    curTime = 0;
                    isRequestingPath = true;
                    RequestPathAsync(transform.position, player.position).Forget();
                }
            }
        }

        private async UniTask RequestPathAsync(Vector3 from, Vector3 to)
        {
            var result = await PathRequest(from, to);

            if (result.success)
            {
                path = result.path;
            }
            else
            {
                path = null;
                rb.velocity = Vector2.zero;
            }

            targetIndex = 0;
            isRequestingPath = false;
        }

        public override void FixedUpdate(float time)
        {
            if (path == null || path.Length == 0 || targetIndex >= path.Length)
                return;


            Vector2 targetPos = path[targetIndex];
            Vector2 dir = (targetPos - (Vector2)transform.position);
            Vector2 forward = (targetIndex + 1 < path.Length) ? path[targetIndex + 1] - path[targetIndex] : player.position - path[targetIndex];
            if (Vector3.Dot(forward, dir) <= 0f)
            {
                targetIndex++;
                return;
            }

            
            Vector2 dirNormalized = dir.normalized;

            if(atkRange >= (player.position - transform.position).magnitude)
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

            // 목표 지점 도착 체크
            if (Vector2.Distance(transform.position, targetPos) < 0.1f)
            {
                targetIndex++;
            }
        }
    }
}

