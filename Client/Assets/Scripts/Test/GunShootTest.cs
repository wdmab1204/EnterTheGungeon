using GameEngine;
using GameEngine.GunController;
using UnityEngine;

public class GunShootTest : MonoBehaviour
{
    GunDataContainer gunDataContainer;
    GunBaseFactory gunFactory;

    public int gunID;
    public Transform shootPoint;

    private GunBase testGun;

    private void Awake()
    {
        InitializeAndReadFileGunData();
        InitializeAndCreateGunObj();
        InitializeGunData();
    }

    public void Shoot()
    {
        var nullablePosition = GameUtility.GetMouseWoirldPosition(Camera.main, Input.mousePosition);
        if (nullablePosition == null)
            return;

        Vector3 dir = nullablePosition.Value - shootPoint.position;
        testGun.Shoot(dir);
    }

    private void InitializeAndReadFileGunData()
    {
        gunDataContainer = new GunDataContainer("gun_data");
        gunDataContainer.ReadFile();
    }

    private void InitializeAndCreateGunObj()
    {
        var obj = Resources.Load<GameObject>("Bullet_1");
        gunFactory = new GunBaseFactory(obj, obj, obj, shootPoint, null);
    }

    private void InitializeGunData()
    {
        GunData gunData = gunDataContainer.Get(gunID);
        testGun = gunFactory.CreateGunBase(gunData.GunForm);
        testGun.Init(gunData);
    }
}
