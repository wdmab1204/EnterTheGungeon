using Cysharp.Threading.Tasks;
using GameEngine.DataSequence.DIContainer;
using GameEngine.Navigation;
using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace GameEngine.Item
{
    public class Coin : BounceObject
    {
        private const float speed = 10f;
        private Vector3[] path;
        private bool isFollowState = false;
        private CancellationTokenSource cancellationTokenSource = new();

        public Transform FollowTarget { get; set; }

        protected override void Start()
        {
            base.Start();
            Physics2D.IgnoreCollision(collider, playerCollider);
        }

        protected override void FixedUpdate()
        {
            if (rb.velocity.sqrMagnitude < 0.5f && FollowTarget != null)
            {
                if (isFollowState == false)
                {
                    rb.velocity = Vector2.zero;
                    rb.gravityScale = 0f;
                    collider.isTrigger = true;
                    Physics2D.IgnoreCollision(collider, playerCollider, false);
                    UpdatePathAsync().Forget();
                    isFollowState = true;
                }
            }
            else
            {
                base.FixedUpdate();
            }
        }

        private async UniTask UpdatePathAsync()
        {
            while (this != null && this.gameObject.IsDestroyed() == false)
            {
                var pathResult = await DIContainer.Resolve<IPathFinder>().FindPathAsync(Transform.position, FollowTarget.position, cancellationTokenSource);

                if (this == null || this.gameObject.IsDestroyed()) break;

                if (pathResult.success && this.path != pathResult.path)
                {
                    this.path = pathResult.path;
                    StopCoroutine(nameof(coFollowTarget));
                    StartCoroutine(nameof(coFollowTarget));
                }

                await UniTask.WaitForSeconds(.3f);
            }
        }

        private IEnumerator coFollowTarget()
        {
            for (int i = 0; i < path.Length; i++)
            {
                var waypoint = path[i];
                while (Vector3.Distance(Transform.position, waypoint) > 0.001f)
                {
                    //rb.velocity = (waypoint - Transform.position).normalized * speed;
                    Transform.position = Vector3.MoveTowards(Transform.position, waypoint, speed * Time.deltaTime);
                    yield return null;
                }
            }

            while(Vector3.Distance(Transform.position, FollowTarget.position) > 0.001f)
            {
                Transform.position = Vector3.MoveTowards(Transform.position, FollowTarget.position, speed * Time.deltaTime);
                yield return null;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        private void OnDrawGizmos()
        {
            if (path == null)
                return;

            for (int i = 0; i < path.Length; i++)
            {
                if (i == 0)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }

        protected override void DoInteractable()
        {
            GameUtility.Destroy(GameObject);
            GameData.Coin.Value += 1;
        }
    }
}

