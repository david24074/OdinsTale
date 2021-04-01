using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private float health = 100;
    private bool finishedRaiding = false;
    private Transform currentTarget;
    private NavMeshAgent navmeshAgent;
    private List<Transform> allBuildings = new List<Transform>();
    private List<Transform> allWalls = new List<Transform>();
    private Transform enemyShip;
    private Vector3 shipLocalPosition;

    public void SetupEnemy(Transform ship, Vector3 oldPosition)
    {
        navmeshAgent = GetComponent<NavMeshAgent>();
        SetAllReachableBuildings();
        FindNewTarget();
        enemyShip = ship;
        shipLocalPosition = oldPosition;
        StartCoroutine(TimeUntilFlee());
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

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            enemyShip.GetComponent<EnemyShip>().SinkShip();
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!navmeshAgent) { return; }

        if (finishedRaiding)
        {
            if (reachedDestination())
            {
                navmeshAgent.enabled = false;
                transform.SetParent(enemyShip);
                transform.position = shipLocalPosition;
                enemyShip.GetComponent<EnemyShip>().PullBack();
            }
        }
        else
        {
            if (currentTarget)
            {
                if (reachedDestination())
                {
                    if (!currentTarget.GetComponent<Burnable>().IsBurning())
                    {
                        currentTarget.GetComponent<Burnable>().ToggleFire(true);
                    }
                }
            }
            else
            {
                SetAllReachableBuildings();
                FindNewTarget();
            }
        }
    }

    private bool reachedDestination()
    {
        if (!navmeshAgent)
        {
            return false;
        }

        if (!navmeshAgent.pathPending)
        {
            if (navmeshAgent.remainingDistance <= navmeshAgent.stoppingDistance)
            {
                if (!navmeshAgent.hasPath || navmeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;

                }
            }
        }
        return false;
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

    private IEnumerator TimeUntilFlee()
    {
        //The enemy will be satisfied if it survived 3 minutes of raiding and it will return to its ship
        yield return new WaitForSeconds(30);
        AbandonCurrentTarget();
        finishedRaiding = true;
        navmeshAgent.SetDestination(enemyShip.transform.position);
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
