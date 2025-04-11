using System;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D myRb;
    private Vector2 movement;
    [SerializeField] private Bullet bulletPrefab;

    public event Action<Vector3> onMove;

    private void Awake()
    {
        myRb = GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        bool isMouseClick = Input.GetMouseButtonDown(0);

        if (movement != Vector2.zero)
            onMove?.Invoke(transform.position);

        if (isMouseClick)
        {
            var mousePosition = Input.mousePosition;
            var myPosition = transform.position;
            var mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector2 shootDirection = (mouseWorldPosition - myPosition).normalized;

            Bullet bullet = Instantiate(bulletPrefab);
            bullet.transform.position = myPosition;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = shootDirection * 8;

            bullet.TargetTag = "Mob";
        }
    }

    void FixedUpdate()
    {
        myRb.velocity = movement.normalized * moveSpeed;

    }
}
