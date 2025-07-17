using System;
using System.Collections;
using UnityEngine;

namespace GameEngine.GunController
{
    public class AutomaticGun : GunBase
    {
        private DateTime lastShootDate;
        private TimeSpan delayTimeSpan;
        private GameObject bulletPrefab;
        private Animator muzzleFlash;
        private Transform transform;

        public AutomaticGun(GameObject bulletPrefab, GameObject muzzleFlash, Transform transform)
        {
            this.bulletPrefab = bulletPrefab;
            this.transform = transform;
            this.muzzleFlash = muzzleFlash.GetComponent<Animator>();
        }

        public void Init(GunData gunData)
        {
            delayTimeSpan = TimeSpan.FromSeconds(gunData.Delay);
        }

        public void MouseDown()
        {
            muzzleFlash.gameObject.SetActive(true);
        }

        public void MouseUp()
        {
            muzzleFlash.gameObject.SetActive(false);
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

                muzzleFlash.Play("Flash", 0, 0f);
                bullet.TargetTag = "Mob";

                lastShootDate = DateTime.UtcNow;
            }
        }
    }
}