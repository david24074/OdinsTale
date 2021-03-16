using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenHouse : MonoBehaviour
{
    [SerializeField] private int maxCitizens = 5;

    private void Start()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().CheckAvailableBeds();
    }

    public int GetMaxCitizens()
    {
        return maxCitizens;
    }
}
