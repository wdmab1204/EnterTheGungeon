using System;
using UnityEngine;
public class BulletRenderService
{
    private BulletDataContainer bulletDataContainer;

    private Mesh mesh;
    private Material material;
    private Matrix4x4[] matrixBuffer;

    public BulletRenderService(BulletDataContainer bulletDataContainer, Mesh mesh, Material material)
    {
        this.bulletDataContainer = bulletDataContainer;
        this.mesh = mesh;
        this.material = material;

        matrixBuffer = new Matrix4x4[bulletDataContainer.Capacity];
    }

    public void Tick(float dt)
    {
        int drawCount = 0;
        Span<BulletObjectData> bullets = bulletDataContainer.GetActiveBullets();
        for (int i = 0; i < bulletDataContainer.Count; i++)
        {
            // 1. 이동
            bullets[i].pos += bullets[i].vel * dt;

            // 4. 드로우 데이터 준비
            matrixBuffer[drawCount] = Matrix4x4.TRS(bullets[i].pos, Quaternion.identity, Vector3.one);
            drawCount++;

            //Debug.DrawRay(_bullets[i].pos, Vector3.up, Color.red);
        }

        RenderBatches(drawCount, matrixBuffer);
    }

    void RenderBatches(int total, Matrix4x4[] matrices)
    {
        int processed = 0;
        while (processed < total)
        {
            int count = Mathf.Min(1023, total - processed);
            // 최적화: 매 프레임 배열 복사가 부담된다면 
            // 실제로는 미리 선언된 임시 배열을 사용하거나 ComputeBuffer를 고려할 수 있습니다.
            Graphics.DrawMeshInstanced(mesh, 0, material, matrices, count, null, UnityEngine.Rendering.ShadowCastingMode.Off, false);
            processed += count;
        }
    }
}