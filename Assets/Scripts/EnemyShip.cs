﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyShip : MonoBehaviour
{
    private Rigidbody rigidbody;
    private bool troopsDeployed = false, troopsDefeated = false;
    [SerializeField] private float movementSpeed = 1;
    [SerializeField] private NavMeshAgent troops;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!rigidbody.isKinematic || !troopsDefeated)
        {
            rigidbody.velocity = transform.right * movementSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Ground" && !troopsDeployed)
        {
            DeployTroops();
        }
    }

    private void DeployTroops()
    {
        troopsDeployed = true;
        rigidbody.isKinematic = true;
        troops.transform.SetParent(null);
        troops.enabled = true;

        Vector3 troopLocalPosition = troops.transform.localPosition;
        Vector3 point;
        if (RandomPoint(transform.position, 3, out point))
        {
            troops.Warp(point);
        }
        troops.GetComponent<Enemy>().SetupEnemy(transform, troopLocalPosition);
    }

    public void PullBack()
    {
        transform.Rotate(transform.up * 180);
        rigidbody.isKinematic = false;
        Destroy(gameObject, 30);
    }

    //Find closest point on the navmesh to spawn the troops on
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    public void SinkShip()
    {
        troopsDefeated = true;
        rigidbody.isKinematic = false;
        Destroy(gameObject, 5);
    }
}
