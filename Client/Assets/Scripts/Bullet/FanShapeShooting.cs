using System;
using UnityEngine;

public class FanShapeShooting : MonoBehaviour
{
    public GameObject bulletPrefab;  // 발사할 탄환 프리팹
    public int bulletCount = 3;      // 발사할 탄환 수
    public float spreadAngle = 60f;  // 부채꼴 각도
    public float bulletSpeed = 5f;   // 탄환 속도

    public void Shoot(Vector2 targetPosition)
    {
        float halfDegree = spreadAngle / 2;
        float startAngle = -halfDegree;
        float angleStep = spreadAngle / (bulletCount - 1);

        float deltaX = transform.position.x - targetPosition.x;
        float deltaY = transform.position.y - targetPosition.y;
        float offsetRadian = Mathf.Atan2(deltaY, deltaX) + Mathf.PI;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector2 shootDirection = 
                new(Mathf.Cos(angle * Mathf.Deg2Rad + offsetRadian),
                    Mathf.Sin(angle * Mathf.Deg2Rad + offsetRadian));
            GameObject go = Instantiate(bulletPrefab);
            var bullet = go.GetComponent<Bullet>();
            go.transform.position = transform.position;

            bullet.Velocity = shootDirection * bulletSpeed;
            bullet.TargetTag = "Player";
            bullet.Range = -1;
            bullet.Damage = 1;
        }
    }
}