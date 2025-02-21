using GameEngine.DataSequence.Random;
using System.Collections.Generic;
using UnityEngine;


public class DTScene : MonoBehaviour
{
    private List<Vector3> points = new();

    [SerializeField] private int count;

    private void Start()
    {
        points.Capacity = count;
        NormalDistribution random = new(new System.Random(), 0, 8);
        for (int i = 0; i < count; i++)
        {
            points.Add(new Vector3());
        }
    }


}
