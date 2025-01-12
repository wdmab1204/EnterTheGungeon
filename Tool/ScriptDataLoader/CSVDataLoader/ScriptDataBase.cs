namespace CSVDataLoader
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

    public class ItemScriptData : ScriptDataBase
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
}
