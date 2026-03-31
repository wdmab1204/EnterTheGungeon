using UnityEngine;

public class BulletPhyscisService
{
    private BulletDataContainer bulletContainer;
    private TagContainer tagContainer;
    private const int maxCollisionCount = 8;
    private Collider2D[] cachedColliders;

    public BulletPhyscisService(BulletDataContainer bulletContainer, TagContainer tagContainer)
    {
        this.bulletContainer = bulletContainer;
        this.cachedColliders = new Collider2D[maxCollisionCount];
        this.tagContainer = tagContainer;
    }

    public void Tick(float dt)
    {
        var datas = bulletContainer.GetActiveBullets();
        for(int i = 0; i < bulletContainer.Count; i++)
        {
            if (TryCollision(datas[i].pos, datas[i].radius, datas[i].targetTagID, datas[i].targetLayerMask))
                bulletContainer.Remove(i);
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
