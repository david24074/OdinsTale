using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Harvestable : MonoBehaviour
{
    private enum resourceTypes { Wood, Stone, Metal };

    [SerializeField] private resourceTypes activeResource;
    [SerializeField] private int resourceHealth = 50;
    [SerializeField] private float jiggleStrength = 2;

    [Header("Resource gain")]
    [SerializeField] private int minAmount = 5;
    [SerializeField] private int maxAmount = 10;
    
    public void DoDamageToResource(int damageAmount)
    {
        transform.DOComplete();
        transform.DOShakeScale(.5f, jiggleStrength, 10, 90, true);
        resourceHealth -= damageAmount;

        if(resourceHealth <= 0)
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ConstructionManager>().AddResource(Random.Range(minAmount, maxAmount),
                activeResource.ToString(),
                GetComponent<JobActivator>());
            Destroy(gameObject);
        }
    }
}
