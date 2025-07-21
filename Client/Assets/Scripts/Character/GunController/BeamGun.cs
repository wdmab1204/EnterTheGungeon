using System;
using UnityEngine;

namespace GameEngine.GunController
{
    public class BeamGun : GunBase
    {
        private GameObject bulletPrefab;
        private Transform transform;
        private BeamRenderer beamRenderer;
        private DateTime lastShootDate;
        private TimeSpan interval = TimeSpan.FromSeconds(0.1f);

        public BeamGun(GameObject bulletPrefab, Transform transform)
        {
            this.bulletPrefab = bulletPrefab;
            this.transform = transform;
        }

        public void Init(GunData gunData)
        {

        }

        public void MouseDown()
        {
            Debug.Log("Mouse Down");
        }

        public void MouseUp()
        {
            Debug.Log("Mouse Up");
        }

        public bool Shoot(Vector3 direction)
        {
            if (DateTime.UtcNow >= lastShootDate + interval)
            {
                //shoot
                Bullet bullet = UnityEngine.Object.Instantiate(bulletPrefab).GetComponent<Bullet>();
                bullet.transform.position = transform.position;
                bullet.Velocity = direction * 8;

                bullet.TargetTag = "Mob";
                
                lastShootDate = DateTime.UtcNow;
                beamRenderer.Add(bullet.transform);

                return true;
            }

            return false;
        }
    }
}