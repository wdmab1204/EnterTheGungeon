using UnityEngine;

public class InteractableController
{
    private Transform transform;
    private float range;

    public InteractableController(Transform transform, float range)
    {
        this.transform = transform;
        this.range = range;
    }

    public void CheckInteractable()
    {
        var hits = Physics2D.CircleCastAll(transform.position, range, Vector2.zero);

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.OnInteractable();
            }
        }
    }

    ~InteractableController()
    {
        // 특별히 해줄 게 없다면 비워둬도 됨
    }
}