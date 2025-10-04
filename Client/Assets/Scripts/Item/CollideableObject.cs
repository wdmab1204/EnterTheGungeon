using GameEngine.DataSequence.DIContainer;
using System.Threading;
using UnityEngine;

namespace GameEngine.Item
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class CollideableObject : MonobehaviourExtension
    {
        protected GameObject player => DIContainer.Resolve<ICharacterController>().GameObject;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject == player)
                DoInteractable();
        }

        protected abstract void DoInteractable();
    }

    public abstract class BounceObject : CollideableObject
    {
        protected Rigidbody2D rb;
        private Vector2 originalPosition;
        private Vector2 previousVelocity;
        private const float bounceDamping = 0.65f;

        protected new BoxCollider2D collider;
        protected BoxCollider2D playerCollider;

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 1f;

            collider = GetComponent<BoxCollider2D>();
            playerCollider = player.GetComponent<BoxCollider2D>();

            originalPosition = Transform.position;
            float randomX = UnityEngine.Random.value > 0.5f ? 1f : -1f;
            rb.velocity = new Vector2(randomX, 5f);
        }

        protected virtual void FixedUpdate()
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
    }
}
