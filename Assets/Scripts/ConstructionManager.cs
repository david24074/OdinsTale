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
        if (currentSelectedBuild)
        {
            Destroy(currentSelectedBuild);
        }

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

            if (Input.GetButtonDown("Fire1"))
            {
                PlaceDownBuilding();   
            }
            if (Input.GetButtonDown("Fire2"))
            {
                Destroy(currentSelectedBuild);
            }
        }
    }

    private void PlaceDownBuilding()
    {
        Transform buildingMesh = currentSelectedBuild.GetComponent<ConstructionBuilding>().GetBuilding().transform;
        RaycastHit[] hits = Physics.BoxCastAll(buildingMesh.position, buildingMesh.localScale / 4, transform.up, transform.rotation, 1);
        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i].transform.tag == "Building")
            {
                Debug.Log("Object was obstructed");
                return;
            }
        }

        buildingMesh.tag = "Building";
        currentSelectedBuild = null;
    }
}
