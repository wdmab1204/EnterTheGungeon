using GameEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameEngine.GunController
{
    public enum GunForm
    {
        Semiautomatic = 1,
        Automatic = 2,
        Charged = 3,
        Beam = 4
    }

    public interface IGunController
    {
        void Equip(int id);
    }

    public class GunController : MonoBehaviour, IGunController
    {
        [SerializeField] SpriteRenderer gunRenderer;
        [SerializeField] Transform body;
        [SerializeField] Transform muzzle;
        [SerializeField] GameObject muzzleFlash;
        [SerializeField] UI_Reload ui_Reload;

        private GunBase currentGun;
        private GunForm currentForm;
        private int myGunID;

        private Camera mainCamera;
        private CameraShake camShake;

        private bool IsReloadEnd = true;
        private float reloadTime = 0f;

        private Dictionary<int, GunData> dataCache = new();

        private void Awake()
        {
            mainCamera = Camera.main;
            camShake = mainCamera.GetComponent<CameraShake>();
            var jsonString = Resources.Load<TextAsset>("gun_data").text;
            foreach (var gunData in JsonUtility.FromJson<GunDataList>(jsonString).guns)
                dataCache.Add(gunData.ID, gunData);
        }

        public void Equip(int id)
        {
            var gunData = dataCache[id];
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

            GameData.EquipGunData = gunData;
            GameData.AmmoSize.Value = gunData.AmmoSize;
            GameData.AmmoCount.Value = gunData.AmmoCount;
            currentGun = gunBase;
            currentForm = gunData.GunForm;
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
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (GameData.AmmoCount.Value == 0)
                return;

            if (Input.GetKeyDown(KeyCode.R))
                IsReloadEnd = false;

            if(IsReloadEnd == false)
            {
                reloadTime += Time.deltaTime;
                ui_Reload.Play(dataCache[myGunID].ReloadTime);
                if (reloadTime >= dataCache[myGunID].ReloadTime)
                {
                    IsReloadEnd = true;
                    reloadTime = 0f;
                    int prevAmmoSize = GameData.AmmoSize.Value;
                    GameData.AmmoSize.Value = 
                        dataCache[myGunID].AmmoCount == -1 ||
                        GameData.AmmoCount.Value > dataCache[myGunID].AmmoSize ?
                        dataCache[myGunID].AmmoSize : 0;
                    GameData.AmmoCount.Value = 
                        dataCache[myGunID].AmmoCount == -1 ? 
                        -1 : GameData.AmmoCount.Value - (dataCache[myGunID].AmmoSize - prevAmmoSize);
                }
                    
                return;
            }

            bool isMouseDown = Input.GetMouseButtonDown(0); // 버튼을 누른 시점
            bool isMouseHold = Input.GetMouseButton(0);     // 버튼을 누르고 있는 동안
            bool isMouseUp = Input.GetMouseButtonUp(0);     // 버튼에서 손을 뗀 시점

            bool isMouseClick = (currentForm == GunForm.Automatic || currentForm == GunForm.Beam)
                                ? isMouseHold
                                : isMouseDown;

            if (currentGun == null)
                return;

            if (isMouseDown)
                currentGun.MouseDown();

            if (isMouseClick)
            {
                var mouseWorldPosition = GameUtility.GetMouseWoirldPosition(mainCamera, Input.mousePosition);
                if (mouseWorldPosition == null)
                    return;
                Vector2 shootDirection = (mouseWorldPosition.Value - body.position).normalized;


                if (GameData.AmmoSize.Value <= 0)
                {
                    IsReloadEnd = false;
                    return;
                }

                // Shoot 호출
                var result = currentGun.Shoot(shootDirection);
                if (result)
                    GameData.AmmoSize.Value -= 1;

                //Camera Shake
                var shakeDuration = dataCache[myGunID].ShakeDuration;
                var shakeIndensity = dataCache[myGunID].ShakeIntensity;
                camShake.Shake(shakeDuration, shakeIndensity);
            }

            if (isMouseUp)
                currentGun.MouseUp();
        }
    }
}
