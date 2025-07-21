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

        private GunData gunData;

        public AutomaticGun(GameObject bulletPrefab, GameObject muzzleFlash, Transform transform)
        {
            this.bulletPrefab = bulletPrefab;
            this.transform = transform;
            this.muzzleFlash = muzzleFlash.GetComponent<Animator>();
        }

        public void Init(GunData gunData)
        {
            this.gunData = gunData;
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

        public bool Shoot(Vector3 direction)
        {
            if (DateTime.UtcNow >= lastShootDate + delayTimeSpan)
            {
                //shoot
                Bullet bullet = UnityEngine.Object.Instantiate(bulletPrefab).GetComponent<Bullet>();
                bullet.transform.position = transform.position;

                bullet.Range = gunData.Range;
                bullet.Knockback = gunData.Knockback;
                bullet.Damage = gunData.Damage;

                float halfSpread = gunData.Spread / 2f;
                float randomAngle = UnityEngine.Random.Range(-halfSpread, halfSpread);
                float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                float spreadAngle = baseAngle + randomAngle;

                direction = new Vector3(Mathf.Cos(spreadAngle * Mathf.Deg2Rad), Mathf.Sin(spreadAngle * Mathf.Deg2Rad));
                bullet.Velocity = direction * gunData.Speed;

                muzzleFlash.Play("Flash", 0, 0f);
                bullet.TargetTag = "Mob";

                lastShootDate = DateTime.UtcNow;

                return true;
            }

            return false;
        }
    }
}