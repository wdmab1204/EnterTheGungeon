using UnityEngine;

namespace GameEngine.GunController
{
    public interface GunBase
    {
        void Shoot(Vector3 direction);
        void Init(GunData data);
    }
}