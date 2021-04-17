using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //Used for most of the static functions
    private static GameManager gameManager;

    [SerializeField] private GameObject citizen, meleeUnit, rangedUnit;

    [Header("Building Settings")]
    [SerializeField] private float objectYPlacement;
    [SerializeField] private GameObject buildingsMenu;
    [SerializeField] private float checkForJobsInterval = 1;
    [SerializeField] private Transform buildingParent, citizenParent;

    [Header("UI settings")]
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI stoneText, foodText, waterText;
    [SerializeField] private TextMeshProUGUI bedsText, citizensText, happinessText, goldText;
    [SerializeField] private MessageLog messageLogger;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private GameObject loseMenu;

    public enum resourceTypes { Wood, Stone, Gold };
    private int currentWoodAmount = 0;
    private int currentStoneAmount = 0;
    private int currentFoodAmount = 0;
    private int currentWaterAmount = 0;
    private int currentBedsAmount = 0;
    private int currentGoldAmount = 0;
    private int currentEmployedAmount = 0;
    private float currentHappinessAmount = 100;

    private int goldPerCitizen = 2;
    private bool taxesEnabled = false;

    private GameObject currentSelectedBuild, currentSelectedUnit;
    private Grid gridObject;

    private List<GameObject> allBuildings = new List<GameObject>();
    private List<Citizen> allCitizens = new List<Citizen>();
    private List<JobActivator> allJobs = new List<JobActivator>();
    private List<UnitSave> allUnits = new List<UnitSave>();

    [Header("Attacking Settings")]
    [SerializeField] private Transform enemyShipInstantiateContent;
    [SerializeField] private GameObject[] enemyShips;
    [SerializeField] private int chanceToSpawnEnemy = 5;
    [SerializeField] private ParticleSystem unitSetGoalParticle;

    [Header("Audio")]
    [SerializeField] private AudioClip[] newJobSounds;
    [SerializeField] private AudioClip attackSound, objectPlaceSound;

    [Header("Time Settings")]
    [SerializeField] private float dayLengthInSeconds = 1200;
    [SerializeField] private TextMeshProUGUI timeText;
    private float currentTimeIndex = 0;
    private int currentDay = 1, currentYear = 0;

    //Current selected building resource cost
    private int woodBuildCost, stoneBuildCost, citizenBuildCost;

    private SaveGame currentSave;

    private void SpawnEnemyShips()
    {
        if (GameObject.FindGameObjectWithTag("Enemy"))
        {
            return;
        }

        AudioManager.PlayAudioClipGlobal(attackSound);
        int maxAmount = Mathf.RoundToInt(allCitizens.Count / 100);
        if (maxAmount < 1) { maxAmount = 1; }

        int amountToSpawn = Random.Range(1, maxAmount);

        MessageLog.AddNewMessage("ATTACK! " + amountToSpawn + " enemy ships have been spotted, get ready!");

        List<Transform> randomSpawnPoints = new List<Transform>();

        int childCount = enemyShipInstantiateContent.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            randomSpawnPoints.Add(enemyShipInstantiateContent.GetChild(i));
        }

        for (int i = 0; i < amountToSpawn; i++)
        {
            Transform randomSpawnPoint = randomSpawnPoints[Random.Range(0, randomSpawnPoints.Count)];
            randomSpawnPoints.Remove(randomSpawnPoint);

            GameObject newShip = Instantiate(enemyShips[Random.Range(0, enemyShips.Length)], randomSpawnPoint.position, randomSpawnPoint.rotation);
        }
    }

    private void Start()
    {
        gridObject = GetComponent<Grid>();
        StartCoroutine(CheckIfJobsAvailable());
        gameManager = this;

        if (ES3.KeyExists("CurrentSaveName"))
        {
            string filePath = ES3.Load<string>("CurrentSaveName") + ".es3";
            LoadSaveGame(ES3.Load<SaveGame>("SaveGame", filePath));
        }

        if (currentYear > 0) { timeText.text = "Year: " + currentYear + " - Day: " + currentDay; } else { timeText.text = "Day: " + currentDay; }
        citizensText.text = (currentEmployedAmount + allCitizens.Count).ToString();
        CheckAvailableBeds();
    }

    private void Update()
    {
        currentTimeIndex += 1 * Time.deltaTime;

        if (currentTimeIndex >= dayLengthInSeconds)
        {
            currentTimeIndex = 0;
            NewDay();
        }

        if (currentSelectedBuild)
        {
            UpdateBuildingPlacement();
            return;
        }

        if (currentSelectedUnit)
        {
            UpdateSelectedUnit();
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.tag == "Unit")
                {
                    currentSelectedUnit = hit.transform.gameObject;
                    currentSelectedUnit.GetComponent<FriendlyTroops>().SelectUnit();
                    return;
                }

                if (hit.transform.GetComponent<JobActivator>())
                {
                    AddNewJob(hit.transform.GetComponent<JobActivator>());
                }
            }
        }
    }

    private void UpdateSelectedUnit()
    {
        if (currentSelectedUnit)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if(hit.transform.tag == "Enemy")
                    {
                        currentSelectedUnit.GetComponent<FriendlyTroops>().SetNewTarget(hit.transform);
                        Instantiate(unitSetGoalParticle, hit.transform.position, Quaternion.identity);
                        return;
                    }
                    Vector3 newPos = gridObject.GetNearestPointOnGrid(new Vector3(hit.point.x, objectYPlacement, hit.point.z));
                    Instantiate(unitSetGoalParticle, newPos + Vector3.up * 0.2f, Quaternion.identity);
                    currentSelectedUnit.GetComponent<FriendlyTroops>().SetTargetPosition(newPos);
                }
                if (Input.GetButtonDown("Fire2"))
                {
                    currentSelectedUnit.GetComponent<FriendlyTroops>().DeselectUnit();
                    currentSelectedUnit = null;
                }
            }
        }
    }

    public static bool UnitSelected()
    {
        if (gameManager.currentSelectedUnit) { return true; } else { return false; }
    }

    public static List<GameObject> GetBuildings()
    {
        return gameManager.allBuildings;
    }

    public static GameManager GetManager()
    {
        return gameManager;
    }

    public bool CheckResourcesForBuild(int woodAmount, int stoneAmount, int citizenAmount)
    {
        bool canBePlaced = true;
        if (woodAmount > currentWoodAmount) { canBePlaced = false; }
        if (stoneAmount > currentStoneAmount) { canBePlaced = false; }
        if (citizenAmount > allCitizens.Count) { canBePlaced = false; }

        if (canBePlaced)
        {
            currentWoodAmount -= woodAmount;
            currentStoneAmount -= stoneAmount;
            currentEmployedAmount += citizenAmount;
            DeleteCitizen(citizenAmount);

            woodText.text = currentWoodAmount.ToString();
            stoneText.text = currentStoneAmount.ToString();
            foodText.text = currentFoodAmount.ToString();
            waterText.text = currentWaterAmount.ToString();
            citizensText.text = (currentEmployedAmount + allCitizens.Count).ToString();
        }
        else
        {
            MessageLog.SetNotificationMessage("Not enough resources", 5);
        }

        return canBePlaced;
    }

    private void DeleteCitizen(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            if (allCitizens[0].HasActiveJob()) { allCitizens[0].QuitJob(); }
            currentSave.AllCitizens.Remove(GetCititzenSaveByID(allCitizens[0].gameObject.GetComponent<ObjectID>().GetID()));
            DeleteCitizenFromList(allCitizens[0].gameObject);
            // Destroy(GetCitizenByID(allCitizens[i].gameObject.GetComponent<ObjectID>().GetID()));
        }
    }

    public void ToggleTaxesEnabled(bool b)
    {
        taxesEnabled = b;
    }

    private void DeleteCitizenFromList(GameObject citizen)
    {
        for(int i = 0; i < allCitizens.Count; i++)
        {
            if(allCitizens[i].gameObject == citizen)
            {
                allCitizens.Remove(allCitizens[i]);
                Destroy(citizen);
            }
        }
    }

    private CitizenSave GetCititzenSaveByID(int id)
    {
        for(int i = 0; i < currentSave.AllCitizens.Count; i++)
        {
            if(currentSave.AllCitizens[i].CitizenID == id)
            {
                return currentSave.AllCitizens[i];
            }
        }
        return null;
    }

    public static void RemoveBuildingFromSave(int buildingID)
    {
        SaveGame tempSave = gameManager.currentSave;

        for(int i = 0; i < tempSave.AllBuildings.Count; i++)
        {
            if(buildingID == tempSave.AllBuildings[i].BuildingID)
            {
                tempSave.AllBuildings.Remove(tempSave.AllBuildings[i]);
            }
        }
        gameManager.allBuildings.Remove(gameManager.GetBuildingByID(buildingID));

        gameManager.currentSave = tempSave;
    }

    private void SpawnNewCitizen(CitizenSave optionalSave = default)
    {
        GameObject newCitizen = Instantiate(citizen, transform.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2)), transform.rotation);
        newCitizen.transform.SetParent(citizenParent);
        allCitizens.Add(newCitizen.GetComponent<Citizen>());

        if (optionalSave == null)
        {
            CitizenSave newSave = new CitizenSave();
            newSave.CitizenPosition = newCitizen.transform.position;
            newSave.CitizenRotation = newCitizen.transform.rotation;
            newSave.CurrentJobID = 0;
            newSave.isHappy = true;
            newSave.CitizenID = GetRandomID();
            newCitizen.GetComponent<ObjectID>().SetID(newSave.CitizenID);
            currentSave.AllCitizens.Add(newSave);
            SaveTheGame(currentSave);
            return;
        }

        newCitizen.transform.position = optionalSave.CitizenPosition;
        newCitizen.transform.rotation = optionalSave.CitizenRotation;
        newCitizen.GetComponent<ObjectID>().SetID(optionalSave.CitizenID);
        newCitizen.GetComponent<Citizen>().ToggleHappy(optionalSave.isHappy);
        if(optionalSave.CurrentJobID != 0)
        {
            newCitizen.GetComponent<Citizen>().GiveNewJob(GetBuildingByID(optionalSave.CurrentJobID).GetComponent<JobActivator>());
        }
        citizensText.text = (currentEmployedAmount + allCitizens.Count).ToString();
    }

    private void LoadSaveGame(SaveGame save)
    {
        currentSave = save;
        currentHappinessAmount = currentSave.AmountHappiness;
        currentFoodAmount = currentSave.AmountFood;
        currentStoneAmount = currentSave.AmountStone;
        currentWoodAmount = currentSave.AmountWood;
        currentWaterAmount = currentSave.AmountWater;
        currentEmployedAmount = currentSave.CitizenAmount;
        currentGoldAmount = currentSave.AmountGold;
        currentYear = currentSave.Year;
        currentDay = currentSave.Day;

        Camera.main.transform.position = save.CameraPosition;
        Camera.main.transform.rotation = save.CameraRotation;

        woodText.text = currentWoodAmount.ToString();
        stoneText.text = currentStoneAmount.ToString();
        foodText.text = currentFoodAmount.ToString();
        waterText.text = currentWaterAmount.ToString();
        goldText.text = currentGoldAmount.ToString();

        for (int i = 0; i < currentSave.AllBuildings.Count; i++)
        {
            GameObject newObject = Instantiate(Resources.Load("Buildings/" + currentSave.AllBuildings[i].BuildingName) as GameObject);
            newObject.transform.position = currentSave.AllBuildings[i].BuildingPosition;
            newObject.transform.rotation = currentSave.AllBuildings[i].BuildingRotation;
            allBuildings.Add(newObject);
            newObject.transform.SetParent(buildingParent);
            newObject.GetComponent<ObjectID>().SetID(currentSave.AllBuildings[i].BuildingID);

            if (currentSave.AllBuildings[i].BuildFinished)
            {
                if (newObject.GetComponent<ConstructionBuilding>())
                {
                    if (newObject.GetComponent<ConstructionBuilding>().GetTaxesEnabled())
                    {
                        ToggleTaxesEnabled(true);
                    }
                    Destroy(newObject.GetComponent<ConstructionBuilding>());
                }
                if (newObject.GetComponent<CitizenHouse>())
                {
                    newObject.GetComponent<CitizenHouse>().enabled = true;
                }
                if (newObject.GetComponent<ResourceGenerator>())
                {
                    newObject.GetComponent<ResourceGenerator>().StartGenerator(currentSave.AllBuildings[i].Progress);
                }
                else
                {
                    Destroy(newObject.GetComponent<JobActivator>());
                }
            }
            else
            {
                newObject.GetComponent<ConstructionBuilding>().PlaceBuilding();
                newObject.GetComponent<ConstructionBuilding>().BuildObject(Mathf.RoundToInt(currentSave.AllBuildings[i].Progress));
            }
        }

        for(int i = 0; i < currentSave.AllJobs.Count; i++)
        {
            AddNewJob(GetBuildingByID(currentSave.AllJobs[i]).GetComponent<JobActivator>(), true);
        }

        for (int i = 0; i < currentSave.AllCitizens.Count; i++)
        {
            SpawnNewCitizen(currentSave.AllCitizens[i]);
        }

        for (int i = 0; i < currentSave.AllUnits.Count; i++)
        {
            if (currentSave.AllUnits[i].IsMelee)
            {
                SpawnNewMeleeUnit(Vector3.zero, currentSave.AllUnits[i]);
            }
            else
            {
                SpawnNewRangedUnit(Vector3.zero, currentSave.AllUnits[i]);
            }
        }

        for (int i = 0; i < currentSave.MessageLogMessages.Count; i++)
        {
            MessageLog.AddNewMessage(currentSave.MessageLogMessages[i]);
        }

        //If there are no citizens it means the player started a new game, if this happens lets give him 3 citizens and some food to start off with.
        if (allCitizens.Count < 1)
        {
            for (int i = 0; i < 3; i++)
            {
                SpawnNewCitizen();
            }
        }

        CheckAvailableBeds();
        CheckHappiness();
    }

    public void SpawnNewMeleeUnit(Vector3 optionalPosition = default, UnitSave optionalSave = default)
    {
        GameObject newUnit;
        if (optionalPosition == null) { newUnit = Instantiate(meleeUnit); } else { newUnit = Instantiate(meleeUnit, optionalPosition, Quaternion.identity); }

        newUnit.transform.SetParent(citizenParent);
        if (optionalSave != null)
        {
            newUnit.GetComponent<FriendlyTroops>().SetData(optionalSave);
            allUnits.Add(optionalSave);
            return;
        }

        UnitSave newSave = new UnitSave();
        allUnits.Add(newSave);
        newSave.CurrentHealth = 100;
        newSave.IsMelee = true;
        newSave.UnitPosition = newUnit.transform.position;
        newSave.UnitID = GetRandomID();
        newSave.UnitRotation = newUnit.transform.rotation;
        newUnit.GetComponent<FriendlyTroops>().SetData(newSave);
        currentSave.AllUnits.Add(newSave);
        SaveTheGame(currentSave);
    }

    public void SpawnNewRangedUnit(Vector3 optionalPosition = default, UnitSave optionalSave = default)
    {
        GameObject newUnit;
        if (optionalPosition == null) { newUnit = Instantiate(rangedUnit); } else { newUnit = Instantiate(rangedUnit, optionalPosition, Quaternion.identity); }

        newUnit.transform.SetParent(citizenParent);
        if (optionalSave != null)
        {
            newUnit.GetComponent<FriendlyTroops>().SetData(optionalSave);
            return;
        }

        UnitSave newSave = new UnitSave();
        newSave.CurrentHealth = 100;
        newSave.IsMelee = false;
        newSave.UnitPosition = newUnit.transform.position;
        newSave.UnitID = GetRandomID();
        newSave.UnitRotation = newUnit.transform.rotation;
        newUnit.GetComponent<FriendlyTroops>().SetData(newSave);
        currentSave.AllUnits.Add(newSave);
        SaveTheGame(currentSave);
    }

    public void UpdateBuildingSaveProgress(Vector3 buildingPos, int newProgress)
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
        if (GameObject.FindGameObjectWithTag("Enemy"))
        {
            MessageLog.SetNotificationMessage("Cannot save the game while you are being attacked", 7);
            return;
        }
        if (!loseMenu.activeInHierarchy)
        {
            SaveTheGame(currentSave, true);
        }
    }

    private GameObject GetBuildingByID(int id)
    {
        for(int i = 0; i < buildingParent.childCount; i++)
        {
            if(buildingParent.GetChild(i).GetComponent<ObjectID>().GetID() == id)
            {
                return buildingParent.GetChild(i).gameObject;
            }
        }
        return null;
    }

    private GameObject GetCitizenByID(int id)
    {
        for(int i = 0; i < citizenParent.childCount; i++)
        {
            if (citizenParent.GetChild(i).GetComponent<ObjectID>().GetID() == id)
            {
                return citizenParent.GetChild(i).gameObject;
            }
        }
        return null;
    }

    public void QuitToMenu()
    {
        if (GameObject.FindGameObjectWithTag("Enemy"))
        {
            MessageLog.SetNotificationMessage("Cannot save the game while you are being attacked", 7);
            return;
        }

        SaveTheGame(currentSave, true);
        menuManager.LoadScene(0);
    }

    public void SaveCurrentGame()
    {
        SaveTheGame(currentSave, true);
    }

    private void SaveTheGame(SaveGame saveGame, bool useExtraSaves = default)
    {
        if (GameObject.FindGameObjectWithTag("Enemy"))
        {
            MessageLog.SetNotificationMessage("Cannot save the game while you are being attacked", 7);
            return;
        }

        if (useExtraSaves)
        {
            saveGame.AmountHappiness = currentHappinessAmount;
            saveGame.AmountWood = currentWoodAmount;
            saveGame.AmountStone = currentStoneAmount;
            saveGame.AmountFood = currentFoodAmount;
            saveGame.AmountGold = currentGoldAmount;
            saveGame.AmountWater = currentWaterAmount;
            saveGame.CitizenAmount = currentEmployedAmount;
            saveGame.Day = currentDay;
            saveGame.Year = currentYear;
            saveGame.AllJobs = new List<int>();

            for (int i = 0; i < saveGame.AllBuildings.Count; i++)
            {
                GameObject buildingToSave = GetBuildingByID(saveGame.AllBuildings[i].BuildingID);
                if (buildingToSave.GetComponent<ConstructionBuilding>())
                {
                    saveGame.AllBuildings[i].BuildFinished = false;
                    saveGame.AllBuildings[i].Progress = buildingToSave.GetComponent<ConstructionBuilding>().GetProgress();
                }
                else
                {
                    saveGame.AllBuildings[i].BuildFinished = true;
                    if (buildingToSave.GetComponent<ResourceGenerator>())
                    {
                        saveGame.AllBuildings[i].Progress = buildingToSave.GetComponent<ResourceGenerator>().GetProgress();
                    }
                }

                if (buildingToSave.GetComponent<JobActivator>())
                {
                    if (buildingToSave.GetComponent<JobActivator>().CheckIfJobActive())
                    {
                        saveGame.AllJobs.Add(buildingToSave.GetComponent<ObjectID>().GetID());
                    }
                }
            }

            for (int i = 0; i < saveGame.AllCitizens.Count; i++)
            {
                GameObject citizenToSave = GetCitizenByID(saveGame.AllCitizens[i].CitizenID);
                saveGame.AllCitizens[i].CitizenPosition = citizenToSave.transform.position;
                saveGame.AllCitizens[i].CitizenRotation = citizenToSave.transform.rotation;
                saveGame.AllCitizens[i].isHappy = citizenToSave.GetComponent<Citizen>().IsHappy();
                if(citizenToSave.GetComponent<Citizen>().GetCurrentTarget() != null)
                {
                    saveGame.AllCitizens[i].CurrentJobID = citizenToSave.GetComponent<Citizen>().GetCurrentTarget().GetComponent<ObjectID>().GetID();
                }
                else
                {
                    saveGame.AllCitizens[i].CurrentJobID = 0;
                }
            }

            for (int i = 0; i < saveGame.AllUnits.Count; i++)
            {
                GameObject unitToSave = GetCitizenByID(saveGame.AllUnits[i].UnitID);
                saveGame.AllUnits[i].UnitPosition = unitToSave.transform.position;
                saveGame.AllUnits[i].UnitRotation = unitToSave.transform.rotation;
            }

            saveGame.MessageLogMessages = messageLogger.GetAllMessages();

            saveGame.CameraPosition = Camera.main.transform.position;
            saveGame.CameraRotation = Camera.main.transform.rotation;
        }

        StartCoroutine(TakeScreenshot(saveGame.SaveGameName));
        ES3.Save("SaveGame", saveGame, ES3.Load<string>("CurrentSaveName") + ".es3");
    }

    private IEnumerator TakeScreenshot(string fileName)
    {
        //Wait 1 frame so the ui disables itself before we take a screenshot
        yield return new WaitForEndOfFrame();
        Texture2D newTexture = ScreenCapture.CaptureScreenshotAsTexture();
        ES3.SaveImage(newTexture, fileName + ".png");
        Destroy(newTexture);
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

    //0 = Wood
    //1 = Stone
    //2 = Food
    //3 = Water
    public void AddResource(int amount, int resourceIndex, JobActivator optionalJobRemove = default)
    {
        switch (resourceIndex)
        {
            case 0:
                currentWoodAmount += amount;
                woodText.text = currentWoodAmount.ToString();
                break;
            case 1:
                currentStoneAmount += amount;
                stoneText.text = currentStoneAmount.ToString();
                break;
            case 2:
                currentFoodAmount += amount;
                foodText.text = currentFoodAmount.ToString(); ;
                break;
            case 3:
                currentWaterAmount += amount;
                waterText.text = currentWaterAmount.ToString(); ;
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
                if (allBuildings[i].GetComponent<CitizenHouse>().enabled)
                {
                    currentBedsAmount += allBuildings[i].GetComponent<CitizenHouse>().GetMaxCitizens();
                }
            }
        }

        bedsText.text = currentBedsAmount.ToString();
    }

    public void AddNewJob(JobActivator newJob, bool ignoreAudio = default)
    {
        if (!ignoreAudio) 
        {
            AudioManager.PlayAudioClipGlobal(newJobSounds[Random.Range(0, newJobSounds.Length)]);
        }

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
    public void SpawnNewBuilding(string buildingName, int woodCost, int stoneCost, int citizenCost)
    {
        if (GameObject.FindGameObjectWithTag("Enemy"))
        {
            MessageLog.SetNotificationMessage("Cannot build while you are being attacked!", 7);
            return;
        }

        woodBuildCost = woodCost;
        stoneBuildCost = stoneCost;
        citizenBuildCost = citizenCost;

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

        HandleTaxes();

        if(currentDay > 365)
        {
            currentDay = 1;
            currentYear += 1;
        }

        int allCurrentCitizens = allCitizens.Count + currentEmployedAmount;
        int peopleUnfed = 0;
        for(int i = 0; i < allCurrentCitizens; i++)
        {
            currentFoodAmount -= 1;
            currentWaterAmount -= 1;
            if (currentFoodAmount <= 0)
            {
                currentFoodAmount = 0;
                peopleUnfed += 1;
            }
            else if (currentWaterAmount <= 0)
            {
                peopleUnfed += 1;
            }

            if (currentWaterAmount <= 0) { currentWaterAmount = 0; }
        }

        if (peopleUnfed > 1)
        {
            MessageLog.AddNewMessage(peopleUnfed + " citizens are unhappy because they dont have food or water!");
        }else if(peopleUnfed > 0)
        {
            MessageLog.AddNewMessage(peopleUnfed + " citizen is unhappy because they dont have food or water!");
        }

        int unhappyPeople = peopleUnfed;
        if (unhappyPeople > allCitizens.Count) { unhappyPeople = allCitizens.Count; }
        for (int c = 0; c < allCitizens.Count; c++)
        {
            unhappyPeople -= 1;
            if(unhappyPeople > -1)
            {
                if (allCitizens[c])
                {
                    allCitizens[c].ToggleHappy(false);
                }
            }
            else
            {
                if (allCitizens[c])
                {
                    allCitizens[c].ToggleHappy(true);
                }
            }
        }

        foodText.text = currentFoodAmount.ToString();
        waterText.text = currentWaterAmount.ToString();

        CheckHappiness();

        //We want to give the player a headstart before we start attacking him
        if(currentDay > 10 && allBuildings.Count > 3)
        {
            if(Random.Range(0, 100) <= chanceToSpawnEnemy)
            {
                SpawnEnemyShips();
            }
        }

        if (currentYear > 0) { timeText.text = "Year: " + currentYear + " - Day: " + currentDay; } else { timeText.text = "Day: " + currentDay; }
    }

    private void HandleTaxes()
    {
        if (!taxesEnabled)
        {
            return;
        }

        int allCitizensCount = allCitizens.Count + currentEmployedAmount;
        int goldEarned = 0;

        for(int i = 0; i < allCitizensCount; i++)
        {
            goldEarned += goldPerCitizen;
        }

        currentGoldAmount += goldEarned;

        if(allUnits.Count > 0)
        {
            int goldSpent = 0;
            for (int i = 0; i < allUnits.Count; i++)
            {
                //Units cost 5 gold per warrior, every unit has 9 warriors
                goldSpent += 45;
            }

            currentGoldAmount -= goldSpent;
            if(currentGoldAmount <= 0)
            {
                currentGoldAmount = 0;
                MessageLog.AddNewMessage("Tax report! You earned " + goldEarned.ToString() + " today and you paid your all troops " + goldSpent.ToString() + "! You dont have any money left, your citizens are unhappy");
            }
            else
            {
                MessageLog.AddNewMessage("Tax report! You earned " + goldEarned.ToString() + " today and you paid your all troops " + goldSpent.ToString() + "!");
            }
        }
        else
        {
            MessageLog.AddNewMessage("Tax report! You earned " + goldEarned.ToString() + " from your citizens");
        }

        goldText.text = currentGoldAmount.ToString();
    }

    private void CheckHappiness()
    {
        float happinessPerCitizen = 0;
        if (allCitizens.Count > 0) { happinessPerCitizen = 100 / allCitizens.Count; }
        currentHappinessAmount = 0;

        for(int i = 0; i < allCitizens.Count; i++)
        {
            if (allCitizens[i].IsHappy())
            {
                currentHappinessAmount += happinessPerCitizen;
            }
        }

        if(currentHappinessAmount <= 5)
        {
            if(allCitizens.Count > 0)
            {
                int citizensToLeave = Random.Range(3, allCitizens.Count / 100 * 50);
                if (citizensToLeave > allCitizens.Count) { citizensToLeave = allCitizens.Count; }
                DeleteCitizen(citizensToLeave);
                MessageLog.AddNewMessage("Your people are unhappy! " + citizensToLeave.ToString() + " left your city!");
            }
            else if(currentEmployedAmount > 0)
            {
                int citizensToLeave = Random.Range(3, currentEmployedAmount / 100 * 50);
                if (citizensToLeave > currentEmployedAmount) { citizensToLeave = currentEmployedAmount; }
                currentEmployedAmount -= citizensToLeave;
                MessageLog.AddNewMessage("Your people are unhappy! " + citizensToLeave.ToString() + " left your city!");
            }

            int allCitizensLeft = allCitizens.Count + currentEmployedAmount;
            if(allCitizensLeft <= 0)
            {
                Time.timeScale = 0;
                ES3.DeleteFile(ES3.Load<string>("CurrentSaveName") + ".es3");
                ES3.DeleteFile(ES3.Load<string>("CurrentSaveName") + ".png");
                loseMenu.SetActive(true);
            }
            citizensText.text = allCitizensLeft.ToString();
        }

        happinessText.text = Mathf.RoundToInt(currentHappinessAmount).ToString() + "%";
    }

    private void AddNewCitizens()
    {
        if (GameObject.FindGameObjectWithTag("Enemy"))
        {
            return;
        }

        int allCurrentCitizens = allCitizens.Count + currentEmployedAmount;
        float bedsLeft = currentBedsAmount - allCurrentCitizens;
        if(bedsLeft <= 0) { MessageLog.AddNewMessage("Some people tried to join your town but there were no beds available, perhaps we should build more houses");  return; }

        //How many citizens are added depends on the happiness of the people
        int citizensToAdd = Mathf.RoundToInt(bedsLeft / 100 * currentHappinessAmount);
        for(int i = 0; i < citizensToAdd; i++)
        {
            SpawnNewCitizen();
        }
        if (currentHappinessAmount >= 100) { MessageLog.AddNewMessage("Its a new day and " + citizensToAdd.ToString() + " decided to join your town!"); } else
        {
            float citizensLeft = bedsLeft - citizensToAdd;
            MessageLog.AddNewMessage("Its a new day and " + citizensToAdd.ToString() + " decided to join your town but " + citizensLeft.ToString() + " refused");
        }
        citizensText.text = (currentEmployedAmount + allCitizens.Count).ToString();
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

        if (!CheckResourcesForBuild(woodBuildCost, stoneBuildCost, citizenBuildCost))
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
        newBuildingSave.BuildingID = GetRandomID();
        currentSelectedBuild.GetComponent<ObjectID>().SetID(newBuildingSave.BuildingID);
        newBuildingSave.Progress = 0;
        currentSave.AllBuildings.Add(newBuildingSave);
        SaveTheGame(currentSave);

        currentSelectedBuild.transform.name = currentSelectedBuild.transform.name + " BUILT";
        currentSelectedBuild = null;

        AudioManager.PlayAudioClipGlobal(objectPlaceSound);
    }

    public static int GetRandomID()
    {
        string IDToConvert = "";
        for(int i = 0; i < 6; i++)
        {
            IDToConvert += Random.Range(0, 9);
        }
        return int.Parse(IDToConvert);
    }

    public static float GetDistanceBetween(Vector3 pos1, Vector3 pos2)
    {
        return (pos1 - pos2).sqrMagnitude;
    }

    private void ToggleBuildingsMenu()
    {
        buildingsMenu.SetActive(!buildingsMenu.activeInHierarchy);
    }
}
