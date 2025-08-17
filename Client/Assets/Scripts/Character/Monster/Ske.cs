using GameEngine;
using GameEngine.DataSequence.StateMachine;
using GameEngine.Navigation;
using System;
using System.Collections.Generic;

public class Ske : Monster
{
    protected override (List<UnitState> states, Type defaultState) GetStatesAndDefault()
    {
        var states = new List<UnitState>();
        states.Add(new WalkState(PathFindManager.GetPath, player.transform, 5f, -1));
        return (states, typeof(WalkState));
    }
}
