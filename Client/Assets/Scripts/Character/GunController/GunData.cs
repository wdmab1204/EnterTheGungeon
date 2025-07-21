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
        public int AmmoSize;        //źâ ũ��
        public int AmmoCount;       //ź�෮
        public float ReloadTime;    //������ �ð�
        public int Speed;           //ź��
        public int Knockback;       //�˹�
        public int Damage;          //ȭ��
        public float Delay;         //������
        public int Range;           //��Ÿ�
        public int Spread;          //��ź��

        public int BulletPerShot;   //���߹߻�(���� � ���)

        public float ShakeDuration; //ī�޶� ���� �ð�
        public float ShakeIntensity;//ī�޶� ���� ����

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