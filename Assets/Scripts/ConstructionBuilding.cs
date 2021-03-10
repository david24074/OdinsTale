using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ConstructionBuilding : MonoBehaviour
{
    //We want to move the mesh object downwards when the building is instantiated
    [SerializeField] private float moveDownYLevel, buildHealth = 1000;
    private Transform meshObject;
    [SerializeField] private float amountMoveEachHit;

    private void Start()
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
            transform.DOComplete();
            Destroy(this);
        }
    }

    public GameObject GetBuilding()
    {
        return transform.GetChild(0).gameObject;
    }
}
