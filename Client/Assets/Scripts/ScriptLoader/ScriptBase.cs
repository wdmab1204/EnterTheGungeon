using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptBase
{
    
}

public abstract class ScriptBase<TScriptData> : ScriptBase
    where TScriptData : ScriptDataBase, new()
{
    protected Dictionary<int, TScriptData> map = new Dictionary<int, TScriptData>();
    private byte[] bytes;
    private int index;

    public TScriptData Get(int key)
    {
        return map[key];
    }

    private byte[] LoadFile(string path)
    {
#if UNITY_2022_3_14
        return UnityEngine.Resources.Load<TextAsset>(path).bytes;
#else
        return File.ReadAllBytes(path);
#endif
    }

    protected int ReadInt32()
    {
        var value = BitConverter.ToInt32(bytes, index);
        index += 4;
        return value;
    }

    protected long ReadInt64()
    {
        var value = BitConverter.ToInt64(bytes, index);
        index += 8;
        return value;
    }

    protected byte ReadByte()
    {
        var value = bytes[index];
        index += 1;
        return value;
    }

    protected abstract TScriptData GetData();

    public void LoadScript(string filePath)
    {
        map.Clear();
        index = 0;
        bytes = LoadFile(filePath);
        {
            while (index < bytes.Length)
            {
                var scriptData = GetData();
                map.Add(scriptData.GetKey(), scriptData);
            }
        }
    }
}