using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ConstructionBuilding : MonoBehaviour
{
    //We want to move the mesh object downwards when the building is instantiated
    [SerializeField] private float moveDownYLevel, buildHealth = 100;
    private Transform meshObject;
    private float amountMoveEachHit;
    private bool isObstructed = false;

    public void PlaceBuilding()
    {
        meshObject = transform.GetChild(0);
        meshObject.transform.localPosition = new Vector3(meshObject.localPosition.x, meshObject.localPosition.y - moveDownYLevel, meshObject.localPosition.z);
        amountMoveEachHit = moveDownYLevel / buildHealth;
    }

    public void BuildObject(int amount)
    {
        transform.DOComplete();
        transform.DOShakeScale(.5f, .2f, 10, 90, true);
        buildHealth -= amount;
        meshObject.localPosition = new Vector3(meshObject.localPosition.x, meshObject.localPosition.y + amountMoveEachHit * amount, meshObject.localPosition.z);

        if(buildHealth <= 0)
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().RemoveOldJob(GetComponent<JobActivator>());
            TryHandleComponents();
            transform.DOComplete();
            Destroy(this);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);
    }

    public bool ObjectIsObstructed()
    {
        RaycastHit[] hits = Physics.BoxCastAll(transform.position + GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size / 2, transform.up, Quaternion.identity, 1);

        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i].transform.tag == "Building")
            {
                if(hits[i].transform != transform)
                {
                    Debug.Log(hits[i].transform.name);
                    return true;
                }
            }
        }

        return false;
    }

    private void TryHandleComponents()
    {
        Destroy(GetComponent<JobActivator>());

        if (GetComponent<CitizenHouse>())
        {
            GetComponent<CitizenHouse>().enabled = true;
        }
    }

    public GameObject GetBuilding()
    {
        return transform.GetChild(0).gameObject;
    }
}
