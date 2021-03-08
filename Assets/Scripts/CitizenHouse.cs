using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenHouse : MonoBehaviour
{
    [SerializeField] private int maxCitizens = 5;

    private string buildingID;

    private void Start()
    {
        buildingID = System.Guid.NewGuid().ToString();
    }

    public string GetBuildingID()
    {
        return buildingID;
    }
}
