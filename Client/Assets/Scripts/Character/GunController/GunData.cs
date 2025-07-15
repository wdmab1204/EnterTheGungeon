using System.Collections.Generic;

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
        
    }

    [System.Serializable]
    public class GunDataList
    {
        public List<GunData> guns;
    }
}