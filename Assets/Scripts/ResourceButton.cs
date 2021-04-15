using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceButton : MonoBehaviour
{
    [SerializeField] private string buildingName;
    [SerializeField] private string buildingDescription;
    [Space]
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private GameManager gamemanager;
    [SerializeField] private int woodAmount, stoneAmount, citizenAmount, goldAmount;

    public void SpawnBuilding()
    {
        if (!GameManager.UnitSelected())
        {
            gamemanager.SpawnNewBuilding(buildingName, woodAmount, stoneAmount, citizenAmount);
        }
        else
        {
            MessageLog.SetNotificationMessage("You've still got a unit selected", 5);
        }
    }

    public void SetResources()
    {
        menuManager.SetResourcesMenu(woodAmount, stoneAmount, citizenAmount, goldAmount, buildingDescription);
    }

    public void DisableMenu()
    {
        menuManager.DisableResourceMenu();
    }
}
