using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ChasePlayer : MonoBehaviour
{
    private CharacterController player;
    [SerializeField] private float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<CharacterController>();
    }

    void Update()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.Normalize(); 

        transform.position += direction * speed * Time.deltaTime;
    }
}
