using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTile : MonoBehaviour
{
    private enum objectType { Wall, Road }
    [SerializeField] private objectType activeObjectType;
    [SerializeField] private Transform topChecker, downChecker, leftChecker, rightChecker;
    [SerializeField] private GameObject fourSidesObject, threeSidesObject, twoSidesObject, oneSideObject;

    public void CheckNeighbours()
    {
        bool topActive = CheckIfSideHasNeighbour(topChecker);
        bool downActive = CheckIfSideHasNeighbour(downChecker);
        bool leftActive = CheckIfSideHasNeighbour(leftChecker);
        bool rightActive = CheckIfSideHasNeighbour(rightChecker);

        if (topActive && downActive && leftActive && rightActive)
        {

        }
    }

    public string GetObjectType()
    {
        return activeObjectType.ToString();
    }

    private bool CheckIfSideHasNeighbour(Transform sideChecker)
    {
        RaycastHit[] hits = Physics.BoxCastAll(sideChecker.position, new Vector3(1, 1, 1) / 4, transform.up, Quaternion.identity, 1);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.tag == "Building")
            {
                if (hits[i].transform != transform)
                {
                    if (hits[i].transform.GetComponent<MeshTile>())
                    {
                        if(hits[i].transform.GetComponent<MeshTile>().GetObjectType() == activeObjectType.ToString())
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
}
