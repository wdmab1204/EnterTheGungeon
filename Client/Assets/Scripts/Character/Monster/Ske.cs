using GameEngine;
using GameEngine.DataSequence.StateMachine;
using System;
using System.Collections.Generic;

public class Ske : Monster
{
    protected override (List<UnitState> states, Type defaultState) GetStatesAndDefault()
    {
        var states = new List<UnitState>();
        states.Add(new WalkState
            (
            getDistance: GetDistance,
            speed: 5f,
            atkRange: -1
            ));
        return (states, typeof(WalkState));
    }
}
