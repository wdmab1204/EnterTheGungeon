using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ChasePlayer : MonoBehaviour
{
    private CharacterController player;
    private HashSet<Transform> transformList = new(), removeList = new();
    [SerializeField] private float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<CharacterController>();
    }

    public void AddTransform(Transform transform) => transformList.Add(transform);
    public void RemoveTransform(Transform transform) => transformList.Remove(transform);

    void Update()
    {
        //Vector3 direction = player.transform.position - transform.position;
        //direction.Normalize();

        Vector3 avrPosition = Vector3.zero;
        foreach (Transform t in transformList)
        {
            if(t == null || t.IsDestroyed())
                removeList.Add(t);
            avrPosition += t.position;
        }

        transform.position = avrPosition;

        foreach (var t in removeList)
            transformList.Remove(t);
        removeList.Clear();
    }
}
