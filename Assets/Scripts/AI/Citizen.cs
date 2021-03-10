using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Citizen : MonoBehaviour
{
    private NavMeshAgent navmeshAgent;
    private GameObject citizenHouse;
    private Animator animator;
    private Transform targetObject;

    private void Start()
    {
        navmeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        GameObject.FindGameObjectWithTag("GameManager").GetComponent<ConstructionManager>().AddNewCitizen(this);
    }

    public Transform GetCurrentTarget()
    {
        return targetObject;
    }

    //Mainly used for moving towards new jobs
    public void SetTransformTarget(Transform target)
    {
        navmeshAgent.SetDestination(target.position);
    }

    public void SetVectorTarget(Vector3 target)
    {
        navmeshAgent.SetDestination(target);
    }

    public void QuitJob()
    {
        //Stop the navmesh agent
        navmeshAgent.SetDestination(transform.position);
        targetObject = null;
        //Reset the state machine to its idle state
        animator.SetInteger("JobIndex", 0);
        animator.ResetTrigger("DestinationReached");
    }

    public bool HasActiveJob()
    {
        if(animator.GetInteger("JobIndex") == 0)
        {
            return false;
        }
        return true;
    }

    public void GiveNewJob(JobActivator job)
    {
        job.AddNewWorker(gameObject);
        targetObject = job.transform;
        SetTransformTarget(job.transform);
        animator.SetInteger("JobIndex", job.GetJobIndex());
    }

    //Used for the idle behaviour, we dont want the citizen to move too far away from his house so
    //thats why we select a random position around his home, if he doesnt have a home then just move to a random position around him
    public void SetRandomTarget()
    {
        if (!citizenHouse)
        {
            navmeshAgent.SetDestination(new Vector3(
            transform.position.x + (Random.Range(-1, 1) * 10),
            transform.position.y,
            transform.position.z + (Random.Range(-1, 1) * 10)));
            return;
        }

        navmeshAgent.SetDestination(new Vector3(
            citizenHouse.transform.position.x + (Random.Range(-1, 1) * 10),
            transform.position.y,
            citizenHouse.transform.position.z + (Random.Range(-1, 1) * 10)));
    }

    //We cant call Random.Range in a state machine behaviour so we do it here
    public float GetRandomFloat(float minAmount, float maxAmount)
    {
        return Random.Range(minAmount, maxAmount);
    }
}
