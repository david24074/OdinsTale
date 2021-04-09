using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenHouse : MonoBehaviour
{
    [SerializeField] private int maxCitizens = 5;

    private void Start()
    {
        GameManager.GetManager().CheckAvailableBeds();
    }

    public int GetMaxCitizens()
    {
        return maxCitizens;
    }
}
