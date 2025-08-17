using UnityEngine;

namespace GameEngine.GunController
{
    public class SemiAutomaticGun : GunBase
    {
        private GameObject bulletPrefab;
        private Transform transform;
        private GunData gunData;

        public SemiAutomaticGun(GameObject bulletPrefab, Transform transform)
        {
            this.bulletPrefab = bulletPrefab;
            this.transform = transform;
        }

        public void Init(GunData gunData)
        {
            this.gunData = gunData;
        }

        public void MouseDown()
        {
        }

        public void MouseUp()
        {
        }

        public bool Shoot(Vector3 direction)
        {
            //shoot
            Bullet bullet = UnityEngine.Object.Instantiate(bulletPrefab).GetComponent<Bullet>();
            bullet.transform.position = transform.position;
            bullet.Velocity = direction * 8;

            bullet.Range = gunData.Range;
            bullet.Knockback = gunData.Knockback;
            bullet.Damage = gunData.Damage;

            bullet.TargetTag = "Mob";

            return true;
        }
    }
}