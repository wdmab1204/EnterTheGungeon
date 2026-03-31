using UnityEngine;

public class BulletPhisycsTest : MonoBehaviour
{
    private BulletRenderService renderService;
    private BulletPhyscisService physicsService;
    private BulletDataContainer bulletContainer;
    private TagContainer tagContainer;
    private Mesh mesh;

    public int count;
    public Material material;

    private void Awake()
    {
        InitializeTagContainer();
        InitializeDatas();
        InitializeMesh();
        renderService = new BulletRenderService(bulletContainer, mesh, material);
        physicsService = new BulletPhyscisService(bulletContainer, tagContainer);

    }

    private void Update()
    {
        var dt = Time.deltaTime;
        renderService.Tick(dt);
        physicsService.Tick(dt);
    }

    void InitializeDatas()
    {
        Camera cam = Camera.main;

        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(count)); // 정사각형 그리드

        var bullets = new BulletObjectData[gridSize * gridSize];
        var matrices = new Matrix4x4[gridSize * gridSize];
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

                bullets[spawned] = new BulletObjectData(worldPos, Vector3.one, 1f, tagContainer.GetID("Collideable"), LayerMask.GetMask("Collideable"));

                spawned++;
            }
        }

        bulletContainer = new BulletDataContainer(bullets, gridSize * gridSize);
    }

    void InitializeTagContainer()
    {
        System.Collections.Generic.Dictionary<int, string> container = new()
        {
            {1, "Collideable" },
        };

        tagContainer = new TagContainer(container);
    }

    void InitializeMesh()
    {
        GameObject tempQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        mesh = tempQuad.GetComponent<MeshFilter>().sharedMesh;
        int[] tris = new int[] { 0, 2, 1, 2, 3, 1 }; // Quad 예시
        mesh.triangles = tris;
        Destroy(tempQuad); // 메시는 남고 객체만 삭제
    }
}