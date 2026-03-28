using Unity.Profiling;
using UnityEngine;
using UnityEngine.AI;

public class FollowTarget : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform target;
    private float curT;
    public static ProfilerMarker pathFindMarker = new("Ske NavMesh");

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        curT += Time.deltaTime;
        if(curT > .3f)
        {
            using (pathFindMarker.Auto())
            {
                agent.SetDestination(target.position);
            }
            curT = 0;
        }
        
    }
}
