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
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(TargetTag))
            Debug.LogWarning($"{nameof(TargetTag)} string is empty. please check it");
        if (Damage <= 0)
            Debug.LogWarning($"{nameof(Damage)} is zero or negative integer. please check it");
        if (Knockback < 0)
            Debug.LogWarning($"{nameof(Knockback)} is negative integer. please check it");
        if (Range == 0)
            Debug.LogWarning($"{nameof(Range)} is zero. please check it");
#endif

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
            GameUtility.Destroy(gameObject);

        if (!string.IsNullOrEmpty(TargetTag) && collidableObject.CompareTag(TargetTag))
        {
            //GameUtil.Destroy(collidableObject);
            Rigidbody2D unitRig = collidableObject.GetComponent<Rigidbody2D>();
            unitRig.AddForce(Velocity.normalized * Knockback, ForceMode2D.Impulse);
            GameUtility.Destroy(gameObject);

            UnitAbility ability = collidableObject.GetComponent<UnitAbility>();
            ability.Health.Value -= Damage;
        }   
    }

    private void OnBecameInvisible() 
    {
        GameUtility.Destroy(gameObject);
    }
}
