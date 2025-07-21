using GameEngine;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public string TargetTag;
    public int Damage;
    public int Knockback;
    public int Range;
    public Vector3 Velocity;

    private new Rigidbody2D rigidbody;
    private float timeOfRange;
    private float t = 0f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rigidbody.velocity = Velocity;

        if (Range == -1)
            timeOfRange = float.MaxValue;
        else
            timeOfRange = Range / Velocity.magnitude;
    }

    private void Update()
    {
        if (t < timeOfRange)
        {
            t += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }


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
