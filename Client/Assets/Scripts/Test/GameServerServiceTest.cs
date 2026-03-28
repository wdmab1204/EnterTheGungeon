using GameEngine.GunController;
using System.Collections;
using UnityEngine;

public class GameServerServiceTest : MonoBehaviour
{
    GameServerService gameServerService;
    public string msg;

    void Start()
    {
        gameServerService = new GameServerService();
    }

    public void Connect()
    {
        gameServerService.Connect();
    }

    public void Send()
    {
        gameServerService.Send(msg);
    }

    public void Recv()
    {
        gameServerService.Recv();
    }
}
public class GunBaseFactory
{
    private GameObject semiAutoObj;
    private GameObject autoObj;
    private GameObject beamObj;

    private Transform muzzle;
    private GameObject muzzleFlash;

    public GunBaseFactory(GameObject semiAutoObj, GameObject autoObj, GameObject beamObj, Transform muzzle, GameObject muzzleFlash)
    {
        this.semiAutoObj = semiAutoObj;
        this.autoObj = autoObj;
        this.beamObj = beamObj;

        this.muzzle = muzzle;
        this.muzzleFlash = muzzleFlash;
    }

    public GunBase CreateGunBase(GunForm gunForm)
    {
        GunBase gun = null;
        switch (gunForm)
        {
            case GunForm.Semiautomatic:
                gun = new SemiAutomaticGun(semiAutoObj, muzzle);
                break;
            case GunForm.Automatic:
                gun = new AutomaticGun(autoObj, muzzleFlash, muzzle);
                break;
            case GunForm.Beam:
                gun = new BeamGun(beamObj, muzzle);
                break;
        }

        return gun;
    }
}