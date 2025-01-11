using System;
using System.Collections.Generic;

public static class ScriptLoader
{
    private static Dictionary<Type, ScriptBase> map = new Dictionary<Type, ScriptBase>();
    
    public static void Init()
    {
        QuestScript questScript = new();
        questScript.LoadScript("quest");
        map.Add(questScript.GetType(), questScript);

        MonsterScript monsterScript = new();
        monsterScript.LoadScript("monster");
        map.Add(monsterScript.GetType(), monsterScript);
    }

    public static T GetScript<T>() where T : ScriptBase
    {
        if (map.TryGetValue(typeof(T), out var script) == false)
        {
            UnityEngine.Debug.LogError($"{nameof(T)} script is not valid");
            return null;
        }

        return (T)script;
    }
}
