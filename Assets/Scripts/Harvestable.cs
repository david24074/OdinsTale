using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Harvestable : MonoBehaviour
{
    private enum resourceTypes { Wood, Stone, Metal };

    [SerializeField] private resourceTypes activeResource;
    [SerializeField] private int resourceHealth = 500;

    public void MineResource(int damageAmount)
    {
        transform.DOComplete();
        transform.DOShakeScale(.5f, .2f, 10, 90, true);
        Debug.Log("Hit resource");
        resourceHealth -= damageAmount;

        if(resourceHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
