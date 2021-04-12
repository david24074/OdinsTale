using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleOnClick : MonoBehaviour
{
    [SerializeField] private GameObject objectToToggle;
    [SerializeField] private bool onlyIfBuildingFinished = false;
    [SerializeField] private TroopTrainer requireTroopsNotTraining;

    private void OnMouseDown()
    {
        if (onlyIfBuildingFinished)
        {
            //Only toggle the object if the building has been finished
            if (GetComponent<ConstructionBuilding>())
            {
                return;
            }
        }

        if (requireTroopsNotTraining)
        {
            if (requireTroopsNotTraining.TrainingTroops())
            {
                return;
            }
        }

        objectToToggle.SetActive(!objectToToggle.activeInHierarchy);
    }
}
