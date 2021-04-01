using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private float health = 100;
    private Transform currentTarget;
    private NavMeshAgent navmeshAgent;
    private List<Transform> allBuildings = new List<Transform>();
    private List<Transform> allWalls = new List<Transform>();

    public void SetupEnemy()
    {
        navmeshAgent = GetComponent<NavMeshAgent>();
        SetAllReachableBuildings();
        FindNewTarget();
    }

    private void SetAllReachableBuildings()
    {
        allBuildings.Clear();
        allWalls.Clear();
        List<GameObject> allObjects = new List<GameObject>();
        allObjects = GameManager.GetBuildings();

        for (int i = 0; i < allObjects.Count; i++)
        {
            if (allObjects[i].GetComponent<ResourceGenerator>() || allObjects[i].GetComponent<CitizenHouse>())
            {
                NavMeshPath path = new NavMeshPath();
                navmeshAgent.CalculatePath(allObjects[i].transform.position, path);
                //If the path is reachable
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    allBuildings.Add(allObjects[i].transform);
                }
            }
            else if (allObjects[i].GetComponent<MeshTile>())
            {
                if (allObjects[i].GetComponent<MeshTile>().GetObjectType() == "Wall")
                {
                    NavMeshPath path = new NavMeshPath();
                    navmeshAgent.CalculatePath(allObjects[i].transform.position, path);
                    //If the path is reachable
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        allWalls.Add(allObjects[i].transform);
                    }
                }
            }
        }
    }

    //More optimised than Vector3.distance
    Transform GetClosestObject(List<Transform> allTargets)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        for (int i = 0; i < allTargets.Count; i++)
        {
            Vector3 directionToTarget = allTargets[i].position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = allTargets[i];
            }
        }

        return bestTarget;
    }

    private void Update()
    {
        if (!navmeshAgent) { return; }

        if (currentTarget)
        {
            if (!navmeshAgent.pathPending)
            {
                if (navmeshAgent.remainingDistance <= navmeshAgent.stoppingDistance)
                {
                    if (!navmeshAgent.hasPath || navmeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        if (!currentTarget.GetComponent<Burnable>().IsBurning())
                        {
                            currentTarget.GetComponent<Burnable>().ToggleFire(true);
                        }
                    }
                }
            }
        }
        else
        {
            SetAllReachableBuildings();
            FindNewTarget();
        }
    }

    private void AbandonCurrentTarget()
    {
        if (currentTarget)
        {
            if (currentTarget.GetComponent<Burnable>().IsBurning())
            {
                currentTarget.GetComponent<Burnable>().ToggleFire(false);
            }
        }
    }

    public void FindNewTarget()
    {
        currentTarget = GetClosestObject(allBuildings);
        //If no buildings can be reached because there are walls in the way lets target the walls
        if (currentTarget == null)
        {
            currentTarget = GetClosestObject(allWalls);
        }
        navmeshAgent.SetDestination(currentTarget.position);
        Debug.Log("Enemy target is: " + currentTarget.name);
    }
}
