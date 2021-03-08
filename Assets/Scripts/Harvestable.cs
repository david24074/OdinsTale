using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvestable : MonoBehaviour
{
    private enum resourceTypes { Wood, Stone, Metal };

    [SerializeField] private resourceTypes activeResource;
    [SerializeField] private int resourceHealth = 500;
}
