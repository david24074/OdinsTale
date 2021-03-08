using UnityEngine;

public class JobActivator : MonoBehaviour
{
    [SerializeField] private int jobIndex;
    
    public int GetJobIndex()
    {
        return jobIndex;
    }
}
