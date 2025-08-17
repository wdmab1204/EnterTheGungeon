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
        public int AmmoSize;        //탄창 크기
        public int AmmoCount;       //탄약량
        public float ReloadTime;    //재장전 시간
        public int Speed;           //탄속
        public int Knockback;       //넉백
        public int Damage;          //화력
        public float Delay;         //딜레이
        public int Range;           //사거리
        public int Spread;          //산탄도

        public int BulletPerShot;   //다중발사(샷건 등에 사용)

        public float ShakeDuration; //카메라 진동 시간
        public float ShakeIntensity;//카메라 진동 세기

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