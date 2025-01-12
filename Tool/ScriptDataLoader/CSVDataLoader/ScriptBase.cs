using ByteConverter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace CSVDataLoader
{
    public abstract class ScriptBase<TScriptData>
        where TScriptData : ScriptDataBase, new()
    {
        protected Dictionary<int, TScriptData> map = new Dictionary<int, TScriptData>();

        public TScriptData Get(int key)
        {
            return map[key];
        }

        public void LoadScript(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, false))
            {
                string line;
                //첫줄 버리기(컬럼 개수, 컬럼 타입들)
                streamReader.ReadLine();

                while((line = streamReader.ReadLine()) != null)
                {
                    TScriptData data = new TScriptData();
                    string[] values = line.Split(',');
                    FieldInfo[] fields = typeof(TScriptData).GetFields();
                    
                    Debug.Assert(fields.Length == values.Length);

                    for(int i=0; i< fields.Length; i++)
                    {
                        FieldInfo fieldInfo = fields[i];
                        Type type = fieldInfo.FieldType;

                        fieldInfo.SetValue(data, Convert.ChangeType(values[i], type));
                    }

                    map.Add(data.GetKey(), data);
                }
            }
        }
    }

    public class ItemScript : ScriptBase<ItemScriptData>
    {
        
    }
}
