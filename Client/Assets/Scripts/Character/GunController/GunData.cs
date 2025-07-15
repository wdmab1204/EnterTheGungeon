using System.Collections.Generic;

namespace GameEngine.GunController
{
    [System.Serializable]
    public class GunData
    {
        public int ID;

        public string Name;
        public GunForm GunForm;
        public int AmmoSize;        //źâ ũ��
        public int AmmoCount;       //ź�෮
        public float ReloadTime;    //������ �ð�
        public int Speed;           //ź��
        public int Knockback;       //�˹�
        public int Damage;          //ȭ��
        public float Delay;         //������
        public int Range;           //��Ÿ�
        public int Spread;          //��ź��
        
    }

    [System.Serializable]
    public class GunDataList
    {
        public List<GunData> guns;
    }
}