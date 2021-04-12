using System.Collections.Generic;
using UnityEngine;

public class JobActivator : MonoBehaviour
{
    [Tooltip("0 = Idle, 1 = MineResource, 2 = Build")]
    [SerializeField] private int jobIndex;
    [SerializeField] private int maxJobWorkers = 1;
    [SerializeField] private GameObject jobActiveObject;
    private List<GameObject> currentWorkers = new List<GameObject>();
    
    public int GetJobIndex()
    {
        return jobIndex;
    }

    public void AddNewWorker(GameObject newWorker)
    {
        currentWorkers.Add(newWorker);
    }

    public void ToggleJobActiveObject(bool value)
    {
        jobActiveObject.SetActive(value);
    }

    public bool CheckIfJobActive()
    {
        return jobActiveObject.activeInHierarchy;
    }

    public void RemoveAllWorkers()
    {
        for(int i = 0; i < currentWorkers.Count; i++)
        {
            if (currentWorkers[i])
            {
                currentWorkers[i].GetComponent<Citizen>().QuitJob();
            }
        }
        currentWorkers.Clear();
    }

    public void RemoveWorker(GameObject oldWorker)
    {
        oldWorker.GetComponent<Citizen>().QuitJob();
        currentWorkers.Remove(oldWorker);
    }

    public int GetCurrentWorkers()
    {
        return currentWorkers.Count;
    }

    public int GetMaxJobWorkers()
    {
        return maxJobWorkers;
    }
}
