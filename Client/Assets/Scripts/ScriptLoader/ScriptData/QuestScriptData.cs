public class QuestScriptData : ScriptDataBase
{
    public int id;
    public int type;
    public int value;
    public int targetValue;
    public int rewardId;
    public int rewardValue;

    public override int GetKey()
    {
        return id;
    }
}