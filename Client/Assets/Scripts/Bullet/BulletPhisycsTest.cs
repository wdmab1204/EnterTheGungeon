using UnityEngine;

public class BulletPhisycsTest : MonoBehaviour
{
    private BulletRenderService renderService;
    private BulletPhyscisService physicsService;
    private BulletDataContainer bulletContainer;
    private SpaceSplitService spaceSplitService;
    private TagContainer tagContainer;
    private Mesh mesh;

    public int count;
    public Material material;

    private void Awake()
    {
        InitializeTagContainer();
        InitializeDatas();
        InitializeMesh();
        InitializeSpaceSplitService();
        renderService = new BulletRenderService(bulletContainer, mesh, material);
        physicsService = new BulletPhyscisService(bulletContainer, tagContainer, spaceSplitService);

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
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(count));
        var bullets = new BulletObjectData[count];          // count 크기로
        var matrices = new Matrix4x4[count];
        int spawned = 0;

        Vector3 target = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));
        target.z = 0f;

        for (int y = 0; y < gridSize && spawned < count; y++)
        {
            for (int x = 0; x < gridSize && spawned < count; x++)
            {
                float u = (x + 0.5f) / gridSize;
                float v = (y + 0.5f) / gridSize;
                Vector3 viewportPos = new Vector3(u, v, cam.nearClipPlane);
                Vector3 worldPos = cam.ViewportToWorldPoint(viewportPos);
                worldPos.z = 0f;
                bullets[spawned] = new BulletObjectData(
                    worldPos,
                    (target - worldPos).normalized * 2,
                    1f,
                    tagContainer.GetID("Collideable"),
                    LayerMask.GetMask("Collideable"));
                spawned++;
            }
        }

        bulletContainer = new BulletDataContainer(bullets, count);
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

    void InitializeSpaceSplitService()
    {
        int cellSize = 5;
        int mapSize = 500;
        spaceSplitService = new SpaceSplitService(cellSize);

        int layerMask = LayerMask.GetMask("Collideable");

        for (float x = -mapSize; x < mapSize; x += cellSize)
            for (float y = -mapSize; y < mapSize; y += cellSize)
            {
                Vector2 cellCenter = new Vector2(x + cellSize * 0.5f, y + cellSize * 0.5f);
                if (Physics2D.OverlapBox(cellCenter, Vector2.one * cellSize, 0f, layerMask) != null)
                {
                    spaceSplitService.Add(cellCenter);
                    DebugDraw.Cube(cellCenter, Vector3.one * cellSize, Color.red, 1f);
                }
                    
            }
    }
}