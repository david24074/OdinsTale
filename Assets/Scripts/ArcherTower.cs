using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArcherTower : MonoBehaviour
{
    private List<Enemy> targets = new List<Enemy>();
    [SerializeField] private Transform archerObject;
    [SerializeField] private float rotationSpeed, shootForce = 300;
    [SerializeField] private GameObject arrowToFire;

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
        if(targets.Count > 0)
        {
            direction = (targets[0].transform.position - archerObject.position).normalized;
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
        newArrow.GetComponent<Rigidbody>().AddForce((targets[0].transform.position - archerObject.position) * shootForce);
        Destroy(newArrow, 3);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            if (!targets.Contains(other.GetComponent<Enemy>()))
            {
                targets.Add(other.GetComponent<Enemy>());
            }
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (targets.Contains(other.GetComponent<Enemy>()))
            {
                targets.Remove(other.GetComponent<Enemy>());
            }
        }
    }
}
