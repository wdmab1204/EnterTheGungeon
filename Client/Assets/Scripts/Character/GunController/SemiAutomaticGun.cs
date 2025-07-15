using UnityEngine;

namespace GameEngine.GunController
{
    public class SemiAutomaticGun : GunBase
    {
        private GameObject bulletPrefab;
        private Transform transform;

        public SemiAutomaticGun(GameObject bulletPrefab, Transform transform)
        {
            this.bulletPrefab = bulletPrefab;
            this.transform = transform;
        }

        public void Init(GunData gunData)
        {

        }

        public void Shoot(Vector3 direction)
        {
            //shoot
            Bullet bullet = UnityEngine.Object.Instantiate(bulletPrefab).GetComponent<Bullet>();
            bullet.transform.position = transform.position;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = direction * 8;

            bullet.TargetTag = "Mob";
        }
    }
}