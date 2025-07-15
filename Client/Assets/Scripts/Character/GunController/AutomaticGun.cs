using System;
using UnityEngine;

namespace GameEngine.GunController
{
    public class AutomaticGun : GunBase
    {
        private DateTime lastShootDate;
        private TimeSpan delayTimeSpan;
        private GameObject bulletPrefab;
        private Transform transform;

        public AutomaticGun(GameObject bulletPrefab, Transform transform)
        {
            this.bulletPrefab = bulletPrefab;
            this.transform = transform;
        }

        public void Init(GunData gunData)
        {
            delayTimeSpan = TimeSpan.FromSeconds(gunData.Delay);
        }

        public void Shoot(Vector3 direction)
        {
            if (DateTime.UtcNow >= lastShootDate + delayTimeSpan)
            {
                //shoot
                Bullet bullet = UnityEngine.Object.Instantiate(bulletPrefab).GetComponent<Bullet>();
                bullet.transform.position = transform.position;
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.velocity = direction * 8;

                bullet.TargetTag = "Mob";
                
                lastShootDate = DateTime.UtcNow;
            }
        }
    }
}