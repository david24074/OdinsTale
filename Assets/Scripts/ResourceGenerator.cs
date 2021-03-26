using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ResourceGenerator : MonoBehaviour
{
    private GameManager gameManager;
    private enum resourceTypes { Wood, Stone, Gold };

    [SerializeField] private resourceTypes activeResource;
    [SerializeField] private int generateCountdown;
    [SerializeField] private int minAmount, maxAmount;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void StartGenerator(int beginNumber)
    {
        if (beginNumber != 0) { generateCountdown = beginNumber; }
        StartCoroutine(GenerateResource());
    }

    private IEnumerator GenerateResource()
    {
        yield return new WaitForSeconds(generateCountdown);
        gameManager.AddResource(Random.Range(minAmount, maxAmount), activeResource.ToString());
        transform.DOComplete();
        transform.DOShakeScale(.5f, 0.5f, 10, 90, true);
        StartCoroutine(GenerateResource());
    }
}
