using System.Collections;
using System.Collections.Generic;
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
