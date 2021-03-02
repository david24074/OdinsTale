using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionBuilding : MonoBehaviour
{
    public GameObject GetBuilding()
    {
        return transform.GetChild(0).gameObject;
    }
}
