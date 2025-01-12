namespace BinDataLoader
{
    public abstract class ScriptDataBase
    {
        public abstract int GetKey();

        public override int GetHashCode()
        {
            return GetKey();
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }
    }

    public class TestScriptData : ScriptDataBase
    {
        public int timeStamp;
        public int templateId;
        public long dbID;
        public long userDbId;
        public int amount;

        public override int GetKey()
        {
            return timeStamp;
        }
    }

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

    public class MonsterScriptData : ScriptDataBase
    {
        public int id;
        public int hp;
        public int attack;

        public override int GetKey()
        {
            return id;
        }
    }
}
