using GameEngine.GunController;
using System.Collections.Generic;
using UnityEngine;

public class GunDataContainer
{
    private string path;
    private Dictionary<int, GunData> dataCache = new();

    public GunDataContainer(string path)
    {
        this.path = path;
    }

    public void ReadFile()
    {
        var jsonString = Resources.Load<TextAsset>(path).text;
        foreach (var gunData in JsonUtility.FromJson<GunDataList>(jsonString).guns)
            dataCache.Add(gunData.ID, gunData);
    }

    public GunData Get(int id)
    {
        return dataCache[id];
    }
}