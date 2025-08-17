using System;
using UnityEngine;

namespace GameEngine.GunController
{
    public interface GunBase
    {
        bool Shoot(Vector3 direction);
        void Init(GunData data);
        void MouseUp();
        void MouseDown();
    }
}