using GameEngine.DataSequence.EventBus;
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

        private int ammoSize;
        private int ammoCount;

        private GunDataContainer gunDataContainer;
        private GunBaseFactory gunFactory;
        private void Awake()
        {
            mainCamera = Camera.main;
            camShake = mainCamera.GetComponent<CameraShake>();

            InitializeAndReadFileGunData();
            InitializeAndCreateGunObj();
        }

        private void InitializeAndReadFileGunData()
        {
            gunDataContainer = new GunDataContainer("gun_data");
            gunDataContainer.ReadFile();
        }

        private void InitializeAndCreateGunObj()
        {
            var obj = Resources.Load<GameObject>("Bullet_1");
            gunFactory = new GunBaseFactory(obj, obj, obj, muzzle, muzzleFlash);
        }

        public void Equip(int id)
        {
            var gunData = gunDataContainer.Get(id);
            var gunBase = gunFactory.CreateGunBase(gunData.GunForm);
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

            ammoSize = gunData.AmmoSize;
            ammoCount = gunData.AmmoCount;
            
            EventBus.Publish("AmmoSizeMax", gunData.AmmoSize);
            EventBus.Publish("AmmoSize", gunData.AmmoSize);
            EventBus.Publish("AmmoCount", gunData.AmmoCount);
            //GameData.EquipGunData = gunData;
            //GameData.AmmoSize.Value = gunData.AmmoSize;
            //GameData.AmmoCount.Value = gunData.AmmoCount;
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

            if (ammoCount == 0)
                return;

            if (Input.GetKeyDown(KeyCode.R))
                IsReloadEnd = false;

            if(IsReloadEnd == false || ammoSize <= 0)
            {
                reloadTime += Time.deltaTime;
                ui_Reload.Play(gunDataContainer.Get(myGunID).ReloadTime);
                if (reloadTime >= gunDataContainer.Get(myGunID).ReloadTime)
                {
                    IsReloadEnd = true;
                    reloadTime = 0f;
                    int prevAmmoSize = ammoSize;
                    ammoSize = 
                        gunDataContainer.Get(myGunID).AmmoCount == -1 ||
                        ammoCount >= gunDataContainer.Get(myGunID).AmmoSize ?
                        gunDataContainer.Get(myGunID).AmmoSize : 0;
                    ammoCount = 
                        gunDataContainer.Get(myGunID).AmmoCount == -1 ? 
                        -1 : ammoCount - (gunDataContainer.Get(myGunID).AmmoSize - prevAmmoSize);

                    EventBus.Publish("AmmoSize", ammoSize);
                    EventBus.Publish("AmmoCount", ammoCount);
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


                if (ammoSize <= 0)
                {
                    IsReloadEnd = false;
                    return;
                }

                // Shoot 호출
                var result = currentGun.Shoot(shootDirection);
                if (result)
                {
                    //GameData.AmmoSize.Value -= 1;
                    ammoSize -= 1;
                    EventBus.Publish("AmmoSize", ammoSize);
                }
                    
                    

                //Camera Shake
                var shakeDuration = gunDataContainer.Get(myGunID).ShakeDuration;
                var shakeIndensity = gunDataContainer.Get(myGunID).ShakeIntensity;
                camShake.Shake(shakeDuration, shakeIndensity);
            }

            if (isMouseUp)
                currentGun.MouseUp();
        }
    }
}
