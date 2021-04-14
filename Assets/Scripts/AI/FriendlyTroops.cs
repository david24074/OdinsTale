using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendlyTroops : MonoBehaviour
{
    private Transform currentTarget;
    private Vector3 currentTargetPosition;
    private NavMeshAgent agent;

    private List<Animator> unitAnims = new List<Animator>();
    [SerializeField] private GameObject selectedObject;

    [Header("Attack options")]
    [SerializeField] private bool isMelee;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float health = 100;
    [SerializeField] private float attackInterval = 1;
    [SerializeField] private float damagePerAttack = 10, arrowFireSpeed = 10;
    private float currentAttackInterval;

    private void Start()
    {
        if (!agent) { agent = GetComponent<NavMeshAgent>(); }

        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Animator>())
            {
                unitAnims.Add(transform.GetChild(i).GetComponent<Animator>());
            }
        }
    }

    public void SetData(UnitSave unitSave)
    {
        health = unitSave.CurrentHealth;
        transform.position = unitSave.UnitPosition;
        transform.rotation = unitSave.UnitRotation;
        GetComponent<ObjectID>().SetID(unitSave.UnitID);
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
                    Attack();
                }
            }
        }
    }

    private void Attack()
    {
        currentAttackInterval = attackInterval;
        if (isMelee)
        {
            currentTarget.GetComponent<Enemy>().TakeDamage(damagePerAttack);
        }
        else
        {
            for(int i = 0; i < unitAnims.Count; i++)
            {
                GameObject newArrow = Instantiate(arrowPrefab, unitAnims[i].transform.position + Vector3.up * 0.75f, unitAnims[i].transform.rotation);
                //Make the arrow face forward
                newArrow.transform.Rotate(Vector3.up * 90);
                newArrow.GetComponent<Rigidbody>().AddForce((currentTarget.position - (unitAnims[i].transform.position + Vector3.up * 2)) * arrowFireSpeed);
                //We can pass the damage simply via the name
                newArrow.name = damagePerAttack.ToString();
                Destroy(newArrow, 3);
            }
        }
    }
}
