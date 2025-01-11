using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private TestMapBuilder mapBuilder;
    
    void Start()
    {
        ScriptLoader.Init();
        mapBuilder.Build();
    }
}
