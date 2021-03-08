using UnityEngine;

public class JobActivator : MonoBehaviour
{
    [SerializeField] private int jobIndex;
    [SerializeField] private int maxJobWorkers = 1;
    private int currentWorkerAmount = 0;
    
    public int GetJobIndex()
    {
        return jobIndex;
    }

    public void AddNewWorker()
    {
        currentWorkerAmount++;
    }

    public void RemoveWorker()
    {
        currentWorkerAmount--;
    }

    public int GetCurrentWorkers()
    {
        return currentWorkerAmount;
    }

    public int GetMaxJobWorkers()
    {
        return maxJobWorkers;
    }
}
