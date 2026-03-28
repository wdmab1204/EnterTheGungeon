using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class GameServerService
{
    TcpClient client;
    NetworkStream stream;
    byte[] buffer = new byte[1024];

    public void Connect()
    {
        client = new TcpClient("127.0.0.1", 9000);
        stream = client.GetStream();
        Debug.Log("서버 접속 성공!");
    }

    public void Send(string msg)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);
        stream.Write(data, 0, data.Length);
    }

    public void Recv()
    {
        int len = stream.Read(buffer, 0, buffer.Length);

        if (len > 0)
        {
            string msg = Encoding.UTF8.GetString(buffer, 0, len);
            Debug.Log("받음: " + msg);
        }
    }
}
