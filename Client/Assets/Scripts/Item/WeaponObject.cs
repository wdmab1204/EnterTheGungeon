using System.Collections;
using Unity.Collections;
using UnityEngine;

namespace GameEngine.Item
{
    public interface IItemObject
    {
        int Id { get; }
    }

    public class WeaponObject : BounceObject, IItemObject
    {
        public int Id { get => id; set => id = value; }

        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField, ReadOnly] private int id;

        protected override void Start()
        {
            base.Start();
            //Physics2D.IgnoreCollision(collider, playerCollider, false);
            StartCoroutine(coDelayCollider());
        }

        IEnumerator coDelayCollider()
        {
            Physics2D.IgnoreCollision(collider, playerCollider);
            yield return new WaitForSeconds(1f);
            collider.isTrigger = true;
            Physics2D.IgnoreCollision(collider, playerCollider, false);
        }

        public void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
            var dx = sprite.rect.width / sprite.pixelsPerUnit / 2;
            spriteRenderer.transform.localPosition = new Vector3(-dx, 0, 0);
        }

        protected override void DoInteractable()
        {
            GameUtility.Destroy(gameObject);
        }
    }
}