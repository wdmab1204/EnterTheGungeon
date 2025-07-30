using GameEngine;
using GameEngine.DataSequence.StateMachine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Monster
{
    [SerializeField] private GameObject prefab;

    protected override (List<UnitState> states, Type defaultState) GetStatesAndDefault()
    {
        var states = new List<UnitState>();
        states.Add(new GroupShootState(prefab, transform, player.transform));
        states.Add(new TeleportState(transform, player.transform, GameData.CurrentVisitRoom.Value));
        return (states, typeof(TeleportState));
    }
}
