using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Citizen : MonoBehaviour
{
    private NavMeshAgent navmeshAgent;

    private void Start()
    {
        navmeshAgent = GetComponent<NavMeshAgent>();
    }

    public void SetTransformTarget(Transform target)
    {
        navmeshAgent.SetDestination(target.position);
    }
}
