﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ResourceGenerator : MonoBehaviour
{
    private GameManager gameManager;
    private enum resourceTypes { Wood, Stone, Gold };

    [SerializeField] private resourceTypes activeResource;
    [SerializeField] private float generateCountdown;
    [SerializeField] private int minAmount, maxAmount;
    private float currentTimer;
    private bool generatorActive = false;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void StartGenerator(float beginNumber)
    {
        if (beginNumber != 0)
        {
            currentTimer = beginNumber;
        }
        else
        {
            currentTimer = generateCountdown;
        }
        generatorActive = true;
    }

    public float GetProgress()
    {
        return currentTimer;
    }

    private void FixedUpdate()
    {
        if (generatorActive)
        {
            currentTimer -= 1 * Time.fixedDeltaTime;
            if(currentTimer <= 0)
            {
                currentTimer = generateCountdown;
                gameManager.AddResource(Random.Range(minAmount, maxAmount), activeResource.ToString());
                transform.DOComplete();
                transform.DOShakeScale(.5f, 0.5f, 10, 90, true);
            }
        }
    }
}
