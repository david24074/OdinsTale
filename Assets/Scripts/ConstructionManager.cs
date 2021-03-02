using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionManager : MonoBehaviour
{
    private GameObject currentSelectedBuild;
    private Grid gridObject;

    private void Start()
    {
        gridObject = GetComponent<Grid>();
    }

    //This function is called by a button that uses a string to determine what building to instantiate
    public void SpawnNewBuilding(string buildingName)
    {
        GameObject newObject = Resources.Load("Buildings/" + buildingName) as GameObject;
        currentSelectedBuild = Instantiate(newObject);
    }

    private void Update()
    {
        if (currentSelectedBuild)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Use the ground layer for the raycast
            int layerMask = 1 << 8;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                currentSelectedBuild.transform.position = gridObject.GetNearestPointOnGrid(hit.point);
            }
        }
    }
}
