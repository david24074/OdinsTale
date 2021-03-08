using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionManager : MonoBehaviour
{
    [SerializeField] private float objectYPlacement;
    [SerializeField] private GameObject buildingsMenu;

    private GameObject currentSelectedBuild;
    private Grid gridObject;

    private List<GameObject> allBuildings = new List<GameObject>();

    private void Start()
    {
        gridObject = GetComponent<Grid>();
    }

    public GameObject GetBuildingByID(string id)
    {
        for(int i = 0; i < allBuildings.Count; i++)
        {
            if(allBuildings[i].GetComponent<CitizenHouse>().GetBuildingID() == id)
            {
                return allBuildings[i];
            }
        }
        return null;
    }

    //This function is called by a button that uses a string to determine what building to instantiate
    public void SpawnNewBuilding(string buildingName)
    {
        if (currentSelectedBuild)
        {
            Destroy(currentSelectedBuild);
        }

        buildingsMenu.SetActive(false);
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
                currentSelectedBuild.transform.position = gridObject.GetNearestPointOnGrid(new Vector3(hit.point.x, objectYPlacement, hit.point.z));
            }

            if (Input.GetButtonDown("Fire1"))
            {
                PlaceDownBuilding();   
            }
            if (Input.GetButtonDown("Fire2"))
            {
                Destroy(currentSelectedBuild);
                buildingsMenu.SetActive(true);
            }
        }
    }

    private void PlaceDownBuilding()
    {
        //The main object doesnt have the collider so get the actual building with its mesh and collider
        Transform buildingMesh = currentSelectedBuild.GetComponent<ConstructionBuilding>().GetBuilding().transform;

        //Check if an existing building is already located at this position
        RaycastHit[] hits = Physics.BoxCastAll(buildingMesh.position, buildingMesh.localScale / 4, transform.up, transform.rotation, 1);
        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i].transform.tag == "Building")
            {
                Debug.Log("Object was obstructed");
                return;
            }
        }

        buildingsMenu.SetActive(true);
        buildingMesh.tag = "Building";
        allBuildings.Add(currentSelectedBuild);
        currentSelectedBuild = null;
    }

    private void ToggleBuildingsMenu()
    {
        buildingsMenu.SetActive(!buildingsMenu.activeInHierarchy);
    }
}
