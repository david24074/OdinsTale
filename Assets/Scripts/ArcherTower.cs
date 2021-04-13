using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArcherTower : MonoBehaviour
{
    private Transform target;
    [SerializeField] private Transform archerObject;
    [SerializeField] private float rotationSpeed, shootForce = 300;
    [SerializeField] private GameObject arrowToFire;
    [SerializeField] private float arrowDamage = 20;

    private Quaternion lookRotation;
    private Vector3 direction;
    [SerializeField] private float fireDelay;
    private float fireDelaySave;

    private void Start()
    {
        fireDelaySave = fireDelay;
    }

    private void Update()
    {
        if(target)
        {
            direction = (target.position - archerObject.position).normalized;
            lookRotation = Quaternion.LookRotation(direction);
            archerObject.rotation = Quaternion.Slerp(archerObject.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            fireDelay -= 1 * Time.deltaTime;
            if(fireDelay <= 0)
            {
                fireDelay = fireDelaySave;
                FireProjectile();
            }
        }
    }

    private void FireProjectile()
    {
        transform.DOComplete();
        transform.DOShakeScale(.5f, 0.5f, 10, 90, true);

        GameObject newArrow = Instantiate(arrowToFire, archerObject.transform.position, archerObject.transform.rotation);
        newArrow.GetComponent<Rigidbody>().AddForce((target.position - (archerObject.position + Vector3.up * 1)) * shootForce);
        //We can pass the damage simply via the name
        newArrow.name = arrowDamage.ToString();
        Destroy(newArrow, 3);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            if (!target) { target = other.transform; }
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (other.transform == target) { target = null; }
        }
    }
}
