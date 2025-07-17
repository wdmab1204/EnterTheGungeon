using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.GunController
{
    public enum GunForm
    {
        Semiautomatic = 1,
        Automatic = 2,
        Charged = 3,
        Beam = 4
    }

    public class GunController : MonoBehaviour
    {
        [SerializeField] SpriteRenderer gunRenderer;
        [SerializeField] Transform body;
        [SerializeField] Transform muzzle;
        [SerializeField] GameObject muzzleFlash;

        private GunBase myGun;
        private GunForm gunType;
        private int myGunID;
        private Camera mainCamera;
        private CameraShake camShake;

        private Dictionary<int, GunData> datas = new();

        private void Awake()
        {
            mainCamera = Camera.main;
            camShake = mainCamera.GetComponent<CameraShake>();
            var jsonString = Resources.Load<TextAsset>("gun_data").text;
            var list = JsonUtility.FromJson<GunDataList>(jsonString).guns;
            foreach (var gunData in list)
                datas.Add(gunData.ID, gunData);
        }

        public void Set(int id)
        {
            var gunData = datas[id];
            var gunBase = GetGunBase(gunData.GunForm);
            if(gunBase == null)
            {
                Debug.LogError($"Invalid Gun ID : {id}");
                return;
            }

            gunBase.Init(gunData);

            gunRenderer.sprite = Resources.Load<Sprite>($"GunSprite_{id}");
            gunRenderer.transform.localPosition = gunData.TransformData.Position;
            muzzle.localPosition = gunData.TransformData.MuzzlePosition;
            muzzleFlash.transform.localPosition = gunData.TransformData.MuzzlePosition;

            myGun = gunBase;
            gunType = gunData.GunForm;
            myGunID = id; 
        }

        private GunBase GetGunBase(GunForm gunForm)
        {
            GunBase gun = null;
            switch (gunForm)
            {
                case GunForm.Semiautomatic:
                    gun = new SemiAutomaticGun(Resources.Load<GameObject>("Bullet_1"), muzzle);
                    break;
                case GunForm.Automatic:
                    gun = new AutomaticGun(Resources.Load<GameObject>("Bullet_1"), muzzleFlash, muzzle);
                    break;
                case GunForm.Beam:
                    gun = new BeamGun(Resources.Load<GameObject>("Bullet_1"), muzzle);
                    break;
            }

            return gun;
        }

        private void Update()
        {
            bool isMouseDown = Input.GetMouseButtonDown(0); // 버튼을 누른 시점
            bool isMouseHold = Input.GetMouseButton(0);     // 버튼을 누르고 있는 동안
            bool isMouseUp = Input.GetMouseButtonUp(0);     // 버튼에서 손을 뗀 시점

            bool isMouseClick = (gunType == GunForm.Automatic || gunType == GunForm.Beam)
                                ? isMouseHold
                                : isMouseDown;

            if (isMouseDown)
                myGun?.MouseDown();

            if (isMouseClick)
            {
                var mouseWorldPosition = GameUtil.GetMouseWoirldPosition(mainCamera, Input.mousePosition);
                if (mouseWorldPosition == null)
                    return;
                Vector2 shootDirection = (mouseWorldPosition.Value - body.position).normalized;

                // Shoot 호출
                myGun?.Shoot(shootDirection);

                //Camera Shake
                var shakeDuration = datas[myGunID].ShakeDuration;
                var shakeIndensity = datas[myGunID].ShakeIntensity;
                camShake.Shake(shakeDuration, shakeIndensity);
            }

            if (isMouseUp)
                myGun?.MouseUp();
        }
    }
}
