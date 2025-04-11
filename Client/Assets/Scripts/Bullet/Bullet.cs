using GameEngine;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public string TargetTag;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidableObject = collision.gameObject;

        if (collidableObject.CompareTag("Collideable"))
            GameUtil.Destroy(gameObject);

        if (!string.IsNullOrEmpty(TargetTag) && collidableObject.CompareTag(TargetTag))
        {
            GameUtil.Destroy(collidableObject);
            GameUtil.Destroy(gameObject);
        }   
    }

    private void OnBecameInvisible()
    {
        GameUtil.Destroy(gameObject);
    }
}
