using UnityEngine;

public class BulletRenderTest : MonoBehaviour
{
    public GameObject prefab;
    public int count = 100;

    BulletObjectData[] bullets;
    Matrix4x4[] matrices;

    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        Camera cam = Camera.main;

        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(count)); // 정사각형 그리드

        int spawned = 0;

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                float u = (x + 0.5f) / gridSize;
                float v = (y + 0.5f) / gridSize;

                Vector3 viewportPos = new Vector3(u, v, cam.nearClipPlane);
                Vector3 worldPos = cam.ViewportToWorldPoint(viewportPos);
                worldPos.z = 0f;

                bullets[spawned] = new BulletObjectData
                {
                    pos = worldPos,
                    vel = Vector2.one
                };

                matrices[spawned] = Matrix4x4.TRS(
                    worldPos,
                    Quaternion.identity,
                    Vector3.one
                );

                spawned++;
            }
        }
    }
}