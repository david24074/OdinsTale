using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ConstructionBuilding : MonoBehaviour
{
    //We want to move the mesh object downwards when the building is instantiated
    [SerializeField] private float moveDownYLevel, buildHealth = 1000;
    private Transform meshObject;
    private float amountMoveEachHit;

    private void Start()
    {
        meshObject = transform.GetChild(0);
        meshObject.transform.position = new Vector3(meshObject.position.x, meshObject.position.y + moveDownYLevel, meshObject.position.z);
        amountMoveEachHit = moveDownYLevel / buildHealth;
    }

    public void BuildObject(int amount)
    {
        transform.DOComplete();
        transform.DOShakeScale(.5f, .2f, 10, 90, true);
        buildHealth -= amount;
        meshObject.transform.localPosition = new Vector3(meshObject.transform.localPosition.x, meshObject.transform.localPosition.y + amountMoveEachHit, meshObject.transform.localPosition.z);
    }

    public GameObject GetBuilding()
    {
        return transform.GetChild(0).gameObject;
    }
}
