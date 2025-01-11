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