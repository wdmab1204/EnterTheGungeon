using GameEngine;
using GameEngine.DataSequence.EventBus;
using GameEngine.GunController;
using System;
using UnityEngine;

public interface ICharacterController
{
    Transform Transform { get; }
    GameObject GameObject { get; }
}

public class CharacterController : MonobehaviourExtension, ICharacterController
{
    public float moveSpeed = 5f;
    private Rigidbody2D myRb;
    private GunController myGun;
    private Vector2 movement;
    [SerializeField] private Bullet bulletPrefab;

    //public event Action<Vector3> OnMove;

    private void Awake()
    {
        myRb = GetComponent<Rigidbody2D>();
        myGun = GetComponent<GunController>();
    }

    private void Start()
    {
        myGun.Equip(3);
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement == Vector2.zero)
            return;

        EventBus.Publish("PlayerMove", transform.position);
        //OnMove?.Invoke(transform.position);
    }

    void FixedUpdate()
    {
        myRb.velocity = movement.normalized * moveSpeed;

    }
}
