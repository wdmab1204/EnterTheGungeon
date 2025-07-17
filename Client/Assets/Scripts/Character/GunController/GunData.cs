using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.GunController
{
    [System.Serializable]
    public class GunData
    {
        public int ID;

        public string Name;
        public GunForm GunForm;
        public int AmmoSize;        //≈∫√¢ ≈©±‚
        public int AmmoCount;       //≈∫æ‡∑Æ
        public float ReloadTime;    //¿Á¿Â¿¸ Ω√∞£
        public int Speed;           //≈∫º”
        public int Knockback;       //≥ÀπÈ
        public int Damage;          //»≠∑¬
        public float Delay;         //µÙ∑π¿Ã
        public int Range;           //ªÁ∞≈∏Æ
        public int Spread;          //ªÍ≈∫µµ

        public GunTransformData TransformData;
    }

    [System.Serializable]
    public class GunTransformData
    {
        public Vector3 MuzzlePosition;
        public Vector3 MuzzleFlashPosition;
        public Vector3 Position;
    }

    [System.Serializable]
    public class GunDataList
    {
        public List<GunData> guns;
    }
}