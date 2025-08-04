using UnityEngine;

public class GroupBullet : MonoBehaviour
{
    [HideInInspector]
    public Transform owner;

    private void Update()
    {
        if(transform.childCount == 0 || owner == null)
            Destroy(gameObject);
    }
}
