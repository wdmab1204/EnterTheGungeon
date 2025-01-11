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