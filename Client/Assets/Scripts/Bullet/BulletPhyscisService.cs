using UnityEngine;
public class BulletPhyscisService
{
    private BulletDataContainer bulletContainer;
    private TagContainer tagContainer;
    private const int maxCollisionCount = 8;
    private Collider2D[] cachedColliders;
    private SpaceSplitService spaceSplitService;

    public BulletPhyscisService(BulletDataContainer bulletContainer, TagContainer tagContainer, SpaceSplitService spaceSplitService)
    {
        this.bulletContainer = bulletContainer;
        this.cachedColliders = new Collider2D[maxCollisionCount];
        this.tagContainer = tagContainer;
        this.spaceSplitService = spaceSplitService;
    }

    public void Tick(float dt)
    {
        var datas = bulletContainer.GetActiveBullets();
        for(int i = 0; i < bulletContainer.Count; i++)
        {
            if (spaceSplitService.Contains(datas[i].pos) == false)
                continue;

            if (TryCollision(datas[i].pos, datas[i].radius, datas[i].targetTagID, datas[i].targetLayerMask))
            {
                DebugDraw.Circle(datas[i].pos, datas[i].radius, Color.magenta, segments:32, duration:1f);
                bulletContainer.Remove(i);
            }
        }
    }

    private bool TryCollision(Vector2 position, float radius, int targetTagID, int layerMask)
    {
        int count = UnityEngine.Physics2D.OverlapCircleNonAlloc(position, radius, cachedColliders, layerMask);
        for (int i = 0; i < count; i++)
        {
            Collider2D col = cachedColliders[i];
            if (col.CompareTag(tagContainer.GetTag(targetTagID)))
                return true;
        }

        return false;
    }
}

public static class DebugDraw
{
    public static void Circle(Vector3 center, float radius, Color color,
                              int segments = 32, float duration = 0f)
    {
        float angleStep = 360f / segments;
        Vector3 prev = center + new Vector3(radius, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 next = center + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            );
            Debug.DrawLine(prev, next, color, duration);
            prev = next;
        }
    }

    public static void Cube(Vector3 center, Vector3 size, Color color, float duration = 0f)
    {
        Vector3 half = size * 0.5f;

        Vector3 tl = center + new Vector3(-half.x, half.y, 0f);
        Vector3 tr = center + new Vector3(half.x, half.y, 0f);
        Vector3 bl = center + new Vector3(-half.x, -half.y, 0f);
        Vector3 br = center + new Vector3(half.x, -half.y, 0f);

        Debug.DrawLine(tl, tr, color, duration);
        Debug.DrawLine(tr, br, color, duration);
        Debug.DrawLine(br, bl, color, duration);
        Debug.DrawLine(bl, tl, color, duration);
    }
}