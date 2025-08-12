using GameEngine.Navigation;
using System.Collections;
using UnityEngine;

namespace GameEngine.Item
{
    public class Coin : Interactable
    {
        private Rigidbody2D rb;
        private Vector2 originalPosition;
        private Vector2 previousVelocity;
        private const float bounceDamping = 0.65f;
        private const float initialUpwardForce = 5f;
        private const float speed = 10f;
        private Vector3[] path;
        private bool isFollowState = false;
        private new BoxCollider2D collider;
        private BoxCollider2D playerCollider;
        private WaitForSeconds pathUpdateInterval = new WaitForSeconds(.3f);

        public Transform FollowTarget { get; set; }

        public event PathDelegate PathRequest;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 1f;

            collider = GetComponent<BoxCollider2D>();
            playerCollider = player.GetComponent<BoxCollider2D>();
            Physics2D.IgnoreCollision(collider, playerCollider);

            originalPosition = Transform.position;
            float randomX = UnityEngine.Random.value > 0.5f ? 1f : -1f;
            rb.velocity = new Vector2(randomX, initialUpwardForce);
        }

        private void FixedUpdate()
        {
            if (rb.velocity.sqrMagnitude < 0.5f && FollowTarget != null)
            {
                if (isFollowState == false)
                {
                    rb.velocity = Vector2.zero;
                    rb.gravityScale = 0f;
                    collider.isTrigger = true;
                    Physics2D.IgnoreCollision(collider, playerCollider, false);
                    StartCoroutine(nameof(coUpdatePath));
                    isFollowState = true;
                }
            }
            else
            {
                previousVelocity = rb.velocity;

                if (Transform.position.y < originalPosition.y && rb.velocity.y < 0f)
                {
                    Vector2 v = rb.velocity;
                    v.y *= -1f;
                    v *= bounceDamping;
                    rb.velocity = v;

                    Vector2 pos = Transform.position;
                    pos.y = originalPosition.y;
                    Transform.position = pos;
                }

            }
        }

        private IEnumerator coUpdatePath()
        {
            while (true)
            {
                var pathResult = PathRequest(Transform.position, FollowTarget.position);
                if(this.path != pathResult.path)
                {
                    this.path = pathResult.path;
                    StopCoroutine(nameof(coFollowTarget));
                    StartCoroutine(nameof(coFollowTarget));
                }
                yield return pathUpdateInterval;
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
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var velocity = previousVelocity;
            foreach (var contact in collision.contacts)
            {
                var normal = contact.normal;
                if (normal.x > 0f || normal.x < 0f)
                    velocity.x *= -1;
                if (normal.y > 0f || normal.y < 0f)
                    velocity.y *= -1;
                break;
            }
            rb.velocity = velocity;
        }

        private void OnDrawGizmos()
        {
            //if (path == null)
            //    return;

            //for (int i = 0; i < path.Length; i++)
            //{
            //    if (i == 0)
            //    {
            //        Gizmos.color = Color.white;
            //        Gizmos.DrawLine(transform.position, path[i]);
            //    }
            //    else
            //    {
            //        Gizmos.color = Color.white;
            //        Gizmos.DrawLine(path[i - 1], path[i]);
            //    }
            //}
        }

        protected override void DoInteractable()
        {
            GameUtil.Destroy(GameObject);
            GameData.Coin.Value += 1;
        }
    }
}

