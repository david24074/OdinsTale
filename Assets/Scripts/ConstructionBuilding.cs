using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ConstructionBuilding : MonoBehaviour
{
    //We want to move the mesh object downwards when the building is instantiated
    [SerializeField] private float moveDownYLevel, buildHealth = 100;
    [SerializeField] private Transform[] collisionCheckers;

    [Header("On finish build")]
    [SerializeField] private GameObject[] enableOnFinish;
    [SerializeField] private bool enableTaxesOnFinsish = false;
    private float currentHealth;
    private Transform meshObject;
    private float amountMoveEachHit;
    private bool isObstructed = false;

    public void PlaceBuilding()
    {
        currentHealth = buildHealth;
        meshObject = transform.GetChild(0);
        meshObject.transform.localPosition = new Vector3(meshObject.localPosition.x, meshObject.localPosition.y - moveDownYLevel, meshObject.localPosition.z);
        amountMoveEachHit = moveDownYLevel / currentHealth;
    }

    public void BuildObject(int amount)
    {
        transform.DOComplete();
        transform.DOShakeScale(.5f, .2f, 10, 90, true);
        currentHealth -= amount;
        meshObject.localPosition = new Vector3(meshObject.localPosition.x, meshObject.localPosition.y + amountMoveEachHit * amount, meshObject.localPosition.z);

        if(currentHealth <= 0)
        {
            GameManager.GetManager().RemoveOldJob(GetComponent<JobActivator>());
            if (GetComponent<MeshTile>())
            {
                GetComponent<MeshTile>().enabled = true;
            }
            if (GetComponent<ResourceGenerator>())
            {
                GetComponent<ResourceGenerator>().StartGenerator(0);
            }

            for(int i = 0; i < enableOnFinish.Length; i++)
            {
                enableOnFinish[i].SetActive(true);
            }

            if (enableTaxesOnFinsish)
            {
                GameManager.GetManager().ToggleTaxesEnabled(enableTaxesOnFinsish);
                Debug.Log("Enabled taxes");
            }

            TryHandleComponents();
            transform.DOComplete();
            Destroy(this);
        }
    }

    public bool GetTaxesEnabled()
    {
        return enableTaxesOnFinsish;
    }

    public float GetProgress()
    {
        return buildHealth - currentHealth;
    }

    public bool ObjectIsObstructed()
    {
        for(int i = 0; i < collisionCheckers.Length; i++)
        {
            RaycastHit[] hits = Physics.BoxCastAll(collisionCheckers[i].position, new Vector3(1, 1, 1) / 4, transform.up, Quaternion.identity, 1);

            for (int c = 0; c < hits.Length; c++)
            {
                if (hits[c].transform.tag == "Building")
                {
                    if (hits[c].transform != transform)
                    {
                        Debug.Log(hits[c].transform.name);
                        return true;
                    }
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
