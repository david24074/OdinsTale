using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TroopTrainer : MonoBehaviour
{
    [SerializeField] private Slider timerObj;
    [SerializeField] private float timerLength = 120;
    private float currentTimer = 0;

    private string standardText;
    [SerializeField] private string trainingText;
    [SerializeField] private TextMeshProUGUI barracksText;
    [SerializeField] private GameObject troopsToSpawn, buttonObj;

    private void Start()
    {
        standardText = barracksText.text;
        timerObj.maxValue = timerLength;
    }

    private void OnDisable()
    {
        if (TrainingTroops())
        {
            gameObject.SetActive(true);
            return;
        }
    }

    public void TrainTroops()
    {
        if (TrainingTroops())
        {
            MessageLog.SetNotificationMessage("Already training troops here", 5);
            return;
        }

        timerObj.gameObject.SetActive(true);
        buttonObj.SetActive(false);
        barracksText.text = trainingText;
        currentTimer = 0;
        currentTimer += 1 * Time.deltaTime;
    }

    private void Update()
    {
        if (TrainingTroops())
        {
            currentTimer += 1 * Time.deltaTime;
            timerObj.value = currentTimer;

            if(currentTimer >= timerLength)
            {
                barracksText.text = standardText;
                SpawnTroops();
                buttonObj.SetActive(true);
                timerObj.gameObject.SetActive(false);
            }
        }
    }

    private void SpawnTroops()
    {
        Debug.Log("Spawned troops");
    }

    public bool TrainingTroops()
    {
        return timerObj.gameObject.activeInHierarchy;
    }
}
