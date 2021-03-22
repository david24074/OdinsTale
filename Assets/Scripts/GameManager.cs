using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject citizen;

    [Header("Building Settings")]
    [SerializeField] private float objectYPlacement;
    [SerializeField] private GameObject buildingsMenu;
    [SerializeField] private float checkForJobsInterval = 1;
    [SerializeField] private Transform buildingParent;

    [Header("UI settings")]
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI stoneText, foodText;
    [SerializeField] private TextMeshProUGUI bedsText, citizensText;

    public enum resourceTypes { Wood, Stone, Metal };
    private int currentWoodAmount = 0;
    private int currentStoneAmount = 0;
    private int currentFoodAmount = 0;
    private int currentBedsAmount = 0;
    private float currentHappinessAmount = 100;

    private GameObject currentSelectedBuild;
    private Grid gridObject;

    private List<GameObject> allBuildings = new List<GameObject>();
    private List<Citizen> allCitizens = new List<Citizen>();
    private List<JobActivator> allJobs = new List<JobActivator>();

    [Header("Audio")]
    [SerializeField] private AudioClip[] newJobSounds;

    [Header("Time Settings")]
    [SerializeField] private float dayLengthInSeconds = 1200;
    [SerializeField] private TextMeshProUGUI timeText;
    private float currentTimeIndex = 0;
    private int currentDay = 1, currentYear;

    private SaveGame currentSave;

    private void Start()
    {
        gridObject = GetComponent<Grid>();
        StartCoroutine(CheckIfJobsAvailable());

        if (ES3.KeyExists("CurrentSaveName"))
        {
            string filePath = ES3.Load<string>("CurrentSaveName") + ".es3";
            LoadSaveGame(ES3.Load<SaveGame>("SaveGame", filePath));
        }

        if (currentYear > 0) { timeText.text = "Year: " + currentYear + " - Day: " + currentDay; } else { timeText.text = "Day: " + currentDay; }
        citizensText.text = allCitizens.Count + " Citizens";
        CheckAvailableBeds();
    }

    private void LoadSaveGame(SaveGame save)
    {
        currentSave = save;
        currentHappinessAmount = currentSave.AmountHappiness;
        currentFoodAmount = currentSave.AmountFood;
        currentStoneAmount = currentSave.AmountStone;
        currentWoodAmount = currentSave.AmountWood;
        currentYear = currentSave.Year;
        currentDay = currentSave.Day;

        for(int i = 0; i < currentSave.AllBuildings.Count; i++)
        {
            GameObject newObject = Instantiate(Resources.Load("Buildings/" + currentSave.AllBuildings[i].BuildingName) as GameObject);
            newObject.transform.position = currentSave.AllBuildings[i].BuildingPosition;
            newObject.transform.rotation = currentSave.AllBuildings[i].BuildingRotation;

            if (currentSave.AllBuildings[i].BuildFinished)
            {
                //To be implemented: Job progress
            }
            else
            {
                newObject.GetComponent<ConstructionBuilding>().PlaceBuilding();
                newObject.GetComponent<ConstructionBuilding>().BuildObject(Mathf.RoundToInt(currentSave.AllBuildings[i].Progress));
            }
        }
    }

    public void UpdateBuildingSaveProgress(Vector3 buildingPos, float newProgress)
    {
        for(int i = 0; i < currentSave.AllBuildings.Count; i++)
        {
            //We can identify which buildings save data to overwrite by their positions
            if(currentSave.AllBuildings[i].BuildingPosition == buildingPos)
            {
                currentSave.AllBuildings[i].Progress = newProgress;
                return;
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveTheGame(currentSave);
    }

    private GameObject GetBuildingByPosition(Vector3 pos)
    {
        foreach(Transform child in buildingParent)
        {
            if(child.position == pos)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    private void SaveTheGame(SaveGame saveGame, bool ignoreExtraSave = default)
    {
        if (ignoreExtraSave)
        {
            for (int i = 0; i < saveGame.AllBuildings.Count; i++)
            {
                GameObject buildingToSave = GetBuildingByPosition(saveGame.AllBuildings[i].BuildingPosition);
                if (buildingToSave.GetComponent<ConstructionBuilding>())
                {
                    saveGame.AllBuildings[i].BuildFinished = false;
                    saveGame.AllBuildings[i].Progress = buildingToSave.GetComponent<ConstructionBuilding>().GetProgress();
                }
            }
        }

        ES3.Save("SaveGame", saveGame, ES3.Load<string>("CurrentSaveName") + ".es3");
    }

    //Check if theres jobs available every couple seconds
    private IEnumerator CheckIfJobsAvailable()
    {
        for(int i = 0; i < allJobs.Count; i++)
        {
            AssignSpecificJob(allJobs[i]);
        }
        yield return new WaitForSeconds(checkForJobsInterval);
        StartCoroutine(CheckIfJobsAvailable());
    }

    public void AddResource(int amount, string resourceType, JobActivator optionalJobRemove = default)
    {
        switch (resourceType)
        {
            case "Wood":
                currentWoodAmount += amount;
                woodText.text = currentWoodAmount + " Wood";
                break;
            case "Stone":
                currentStoneAmount += amount;
                stoneText.text = currentStoneAmount + " Stone";
                break;
            case "Food":
                currentFoodAmount += amount;
                foodText.text = currentFoodAmount + " Food";
                break;
        }

        if (optionalJobRemove)
        {
            allJobs.Remove(optionalJobRemove);
        }
    }

    public void CheckAvailableBeds()
    {
        currentBedsAmount = 0;

        for(int i = 0; i < allBuildings.Count; i++)
        {
            if (allBuildings[i].GetComponent<CitizenHouse>())
            {
                currentBedsAmount += allBuildings[i].GetComponent<CitizenHouse>().GetMaxCitizens();
            }
        }

        bedsText.text = currentBedsAmount + " Beds";
    }

    public void AddNewJob(JobActivator newJob)
    {
        AudioManager.PlayAudioClipGlobal(newJobSounds[Random.Range(0, newJobSounds.Length)]);

        for (int i = 0; i < allJobs.Count; i++)
        {
            if(newJob == allJobs[i])
            {
                //If the jobActivator gets clicked again then cancel the job
                newJob.ToggleJobActiveObject(false);
                newJob.RemoveAllWorkers();
                allJobs.Remove(newJob);
                return;
            }
        }

        newJob.ToggleJobActiveObject(true);
        allJobs.Add(newJob);
        AssignSpecificJob(newJob);
    }

    public void RemoveOldJob(JobActivator oldJob)
    {
        oldJob.ToggleJobActiveObject(false);
        oldJob.RemoveAllWorkers();
        allJobs.Remove(oldJob);
    }

    public void AddNewCitizen(Citizen citizen)
    {
        allCitizens.Add(citizen);
    }

    private void AssignSpecificJob(JobActivator job)
    {
        int allocatedJobs = job.GetCurrentWorkers();
        for (int i = 0; i < allCitizens.Count; i++)
        {
            if (allocatedJobs >= job.GetMaxJobWorkers())
            {
                return;
            }
            if (!allCitizens[i].GetComponent<Citizen>().HasActiveJob())
            {
                allocatedJobs++;
                allCitizens[i].GetComponent<Citizen>().GiveNewJob(job);
            }
        }
    }

    //This function is called by a button that uses a string to determine what building to instantiate
    public void SpawnNewBuilding(string buildingName)
    {
        if (currentSelectedBuild)
        {
            Destroy(currentSelectedBuild);
        }

        buildingsMenu.SetActive(false);
        GameObject newObject = Resources.Load("Buildings/" + buildingName) as GameObject;
        currentSelectedBuild = Instantiate(newObject);
    }

    private void NewDay()
    {
        currentDay += 1;

        AddNewCitizens();

        if(currentDay > 365)
        {
            currentDay = 1;
            currentYear += 1;
        }

        if (currentYear > 0) { timeText.text = "Year: " + currentYear + " - Day: " + currentDay; } else { timeText.text = "Day: " + currentDay; }
    }

    private void AddNewCitizens()
    {
        float bedsLeft = currentBedsAmount - allCitizens.Count;
        if(bedsLeft <= 0) { return; }

        //How many citizens are added depends on the happiness of the people
        float citizensToAdd = citizensToAdd = bedsLeft / 100 * currentHappinessAmount;
        for(int i = 0; i < citizensToAdd; i++)
        {
            GameObject newCitizen = Instantiate(citizen, transform.position, Quaternion.identity);
            allCitizens.Add(newCitizen.GetComponent<Citizen>());
        }
        citizensText.text = allCitizens.Count + " Citizens";
    }

    private void Update()
    {
        currentTimeIndex += 1 * Time.deltaTime;

        if(currentTimeIndex >= dayLengthInSeconds)
        {
            currentTimeIndex = 0;
            NewDay();
        }

        if (currentSelectedBuild)
        {
            UpdateBuildingPlacement();
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.GetComponent<JobActivator>())
                {
                    AddNewJob(hit.transform.GetComponent<JobActivator>());
                }
            }
        }
    }

    private void UpdateBuildingPlacement()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Use the ground layer for the raycast
        int layerMask = 1 << 8;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            currentSelectedBuild.transform.position = gridObject.GetNearestPointOnGrid(new Vector3(hit.point.x, objectYPlacement, hit.point.z));
            currentSelectedBuild.transform.position = new Vector3(currentSelectedBuild.transform.position.x, objectYPlacement, currentSelectedBuild.transform.position.z);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentSelectedBuild.transform.Rotate(transform.up * 90);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentSelectedBuild.transform.Rotate(-transform.up * 90);
        }
        if (Input.GetButtonDown("Fire1"))
        {
            PlaceDownBuilding();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Destroy(currentSelectedBuild);
            buildingsMenu.SetActive(true);
        }
    }

    private void PlaceDownBuilding()
    {
        //Check if an existing building is already located at this position
        if (currentSelectedBuild.GetComponent<ConstructionBuilding>().ObjectIsObstructed())
        {
            return;
        }

        buildingsMenu.SetActive(true);
        currentSelectedBuild.GetComponent<ConstructionBuilding>().PlaceBuilding();
        AddNewJob(currentSelectedBuild.GetComponent<JobActivator>());
        allBuildings.Add(currentSelectedBuild);
        currentSelectedBuild.transform.SetParent(buildingParent);

        //Unity names instantiated objects (Clone), lets remove that so its easier to save since the original names dont include (Clone)
        currentSelectedBuild.transform.name = currentSelectedBuild.transform.name.Replace("(Clone)", "").Trim();
        BuildingSave newBuildingSave = new BuildingSave();
        newBuildingSave.BuildingName = currentSelectedBuild.transform.name;
        newBuildingSave.BuildingPosition = currentSelectedBuild.transform.position;
        newBuildingSave.BuildingRotation = currentSelectedBuild.transform.rotation;
        newBuildingSave.BuildFinished = false;
        newBuildingSave.Progress = 0;
        currentSave.AllBuildings.Add(newBuildingSave);
        SaveTheGame(currentSave, true);

        currentSelectedBuild.transform.name = currentSelectedBuild.transform.name + " BUILT";
        currentSelectedBuild = null;
    }

    private void ToggleBuildingsMenu()
    {
        buildingsMenu.SetActive(!buildingsMenu.activeInHierarchy);
    }
}
