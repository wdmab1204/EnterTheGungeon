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
        private GunBase myGun;
        private GunForm gunType;
        private Camera mainCamera;

        private Dictionary<int, GunData> datas = new();

        private void Awake()
        {
            mainCamera = Camera.main;
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

            myGun = gunBase;
            gunType = gunData.GunForm;
        }

        private GunBase GetGunBase(GunForm gunForm)
        {
            GunBase gun = null;
            switch (gunForm)
            {
                case GunForm.Semiautomatic:
                    gun = new SemiAutomaticGun(Resources.Load<GameObject>("Bullet_1"), transform);
                    break;
                case GunForm.Automatic:
                    gun = new AutomaticGun(Resources.Load<GameObject>("Bullet_1"), transform);
                    break;
                case GunForm.Beam:
                    gun = new BeamGun(Resources.Load<GameObject>("Bullet_1"), transform);
                    break;
            }

            return gun;
        }

        private void Update()
        {
            bool isMouseDown = Input.GetMouseButtonDown(0); // 버튼을 누른 시점
            bool isMouseHold = Input.GetMouseButton(0);     // 버튼을 누르고 있는 동안

            bool isMouseClick = (gunType == GunForm.Automatic || gunType == GunForm.Beam)
                                ? isMouseHold
                                : isMouseDown;

            if (isMouseClick)
            {
                var mousePosition = Input.mousePosition;
                var myPosition = transform.position;
                var mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
                Vector2 shootDirection = (mouseWorldPosition - myPosition).normalized;

                // Shoot 호출
                myGun?.Shoot(shootDirection);
            }
        }
    }
}
