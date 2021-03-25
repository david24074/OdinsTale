using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectID : MonoBehaviour
{
    [SerializeField] private int objectID;

    public void SetID(int id)
    {
        objectID = id; 
    }

    public int GetID()
    {
        return objectID;
    }
}
