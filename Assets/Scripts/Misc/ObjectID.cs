using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectID : MonoBehaviour
{
    private string objectID;

    public void SetID(string id)
    {
        objectID = id; 
    }

    public string GetID()
    {
        return objectID;
    }
}
