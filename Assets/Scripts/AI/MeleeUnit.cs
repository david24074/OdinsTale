using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeUnit : MonoBehaviour
{
    private Transform currentTarget;
    private Vector3 currentTargetPosition;
    private NavMeshAgent agent;

    private List<Animator> unitAnims = new List<Animator>();
    [SerializeField] private GameObject selectedObject;

    [Header("Attack options")]
    [SerializeField] private float attackInterval = 1;
    [SerializeField] private float damagePerAttack = 10;
    private float currentAttackInterval;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        for(int i = 0; i < transform.childCount; i++)
        {
            unitAnims.Add(transform.GetChild(i).GetComponent<Animator>());
        }
    }

    public Transform GetCurrentTarget()
    {
        return currentTarget;
    }

    public Vector3 GetCurrentTargetPosition()
    {
        return currentTargetPosition;
    }

    public void SelectUnit()
    {
        selectedObject.SetActive(true);
    }

    public void DeselectUnit()
    {
        selectedObject.SetActive(false);
    }

    public void SetNewTarget(Transform newTarget)
    {
        currentTarget = newTarget;
        currentAttackInterval = attackInterval;
    }

    public void SetTargetPosition(Vector3 newPos)
    {
        if (currentTarget) { currentTarget = null; }
        currentTargetPosition = newPos;
        agent.SetDestination(currentTargetPosition);
    }

    private void Update()
    {
        if (currentTarget)
        {
            agent.SetDestination(currentTarget.position);

            //More optimized than vector3.distance
            if(GameManager.GetDistanceBetween(transform.position, currentTarget.position) <= agent.stoppingDistance * agent.stoppingDistance)
            {
                currentAttackInterval -= 1 * Time.deltaTime;
                if(currentAttackInterval <= 0)
                {
                    currentTarget.GetComponent<Enemy>().TakeDamage(damagePerAttack);
                    currentAttackInterval = attackInterval;
                }
            }
        }
    }
}
