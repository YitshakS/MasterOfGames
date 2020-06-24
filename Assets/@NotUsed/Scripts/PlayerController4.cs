using UnityEngine;
using UnityEngine.AI;

public class PlayetController4 : MonoBehaviour
{

    public Transform target;
    private NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        NavMeshHit hit;
        if (!agent.Raycast(target.position, out hit))
        {
            agent.SetDestination(hit.position);
        }

    }
}
