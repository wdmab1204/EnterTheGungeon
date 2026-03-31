using UnityEngine;

public struct BulletObjectData
{
    public Vector3 pos;
    public Vector3 vel;
    public float radius;
    public int targetTagID;
    public int targetLayerMask;

    public BulletObjectData(Vector3 pos, Vector3 vel, float radius, int targetTagID, int targetLayerMask)
    {
        this.pos = pos;
        this.vel = vel;
        this.radius = radius;
        this.targetTagID = targetTagID;
        this.targetLayerMask = targetLayerMask;
    }
}
