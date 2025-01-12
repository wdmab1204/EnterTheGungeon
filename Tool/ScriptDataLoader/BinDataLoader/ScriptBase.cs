using ByteConverter;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace BinDataLoader
{
    public abstract class ScriptBase<TScriptData> 
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

    public class TestScript : ScriptBase<TestScriptData>
    {
        protected override TestScriptData GetData()
        {
            TestScriptData data = new TestScriptData();
            data.timeStamp = ReadInt32();
            data.templateId = ReadInt32();
            data.dbID = ReadInt64();
            data.userDbId = ReadInt64();
            data.amount = ReadInt32();

            return data;
        }
    }

    public class QuestScript : ScriptBase<QuestScriptData>
    {
        protected override QuestScriptData GetData()
        {
            QuestScriptData data = new QuestScriptData();
            data.id = ReadInt32();
            data.type = ReadInt32();
            data.value = ReadInt32();
            data.targetValue = ReadInt32();
            data.rewardId = ReadInt32();
            data.rewardValue = ReadInt32();
            return data;
        }
    }

    public class MonsterScript : ScriptBase<MonsterScriptData>
    {
        protected override MonsterScriptData GetData()
        {
            MonsterScriptData data = new MonsterScriptData();
            data.id = ReadInt32();
            data.hp = ReadInt32();
            data.attack = ReadInt32();

            return data;
        }
    }
}
