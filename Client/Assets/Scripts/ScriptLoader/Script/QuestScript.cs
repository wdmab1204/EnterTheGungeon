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