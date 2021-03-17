using System.Collections.Generic;
using UnityEngine;

public class MeshTile : MonoBehaviour
{
    private enum objectType { Wall, Road }
    [SerializeField] private objectType activeObjectType;
    [SerializeField] private Transform topChecker, downChecker, leftChecker, rightChecker;
    [SerializeField] private Transform[] allSideCheckers;
    [SerializeField] private GameObject fourSidesObject, threeSidesObject, CornerObject, twoSidesObject, oneSideObject, zeroSideObject;

    public void CheckNeighbours()
    {
        //Zero sides object is the standard object thats active so lets disable this first
        zeroSideObject.SetActive(false);

        bool topActive = CheckIfSideHasNeighbour(topChecker);
        bool downActive = CheckIfSideHasNeighbour(downChecker);
        bool leftActive = CheckIfSideHasNeighbour(leftChecker);
        bool rightActive = CheckIfSideHasNeighbour(rightChecker);

        int connectedSides = 0;

        if (topActive) { connectedSides += 1; }
        if (downActive) { connectedSides += 1; }
        if (leftActive) { connectedSides += 1; }
        if (rightActive) { connectedSides += 1; }

        switch (connectedSides)
        {
            case 4:
                fourSidesObject.SetActive(true);
                return;
            case 3:
                threeSidesObject.SetActive(true);
                transform.LookAt(GetMissingNeighbour(topActive, downActive, rightActive, leftActive));
                return;
            case 2:
                if (topActive && downActive) { twoSidesObject.SetActive(true); LookAtNeighbour(topChecker); return; }
                if (leftActive && rightActive) { twoSidesObject.SetActive(true); LookAtNeighbour(leftChecker); return; }

                if (leftActive && topActive) { CornerObject.SetActive(true); LookAtNeighbour(topChecker); return; }
                if (leftActive && downActive) { CornerObject.SetActive(true); LookAtNeighbour(downChecker); return; }
                if (rightActive && topActive) { CornerObject.SetActive(true); LookAtNeighbour(topChecker); return; }
                if (rightActive && downActive) { CornerObject.SetActive(true); LookAtNeighbour(downChecker); return; }
                return;
            case 1:
                oneSideObject.SetActive(true);
                LookAtNeighbour(GetSingularSide());
                return;
            case 0:
                zeroSideObject.SetActive(true);
                return;
        }
    }

    private Transform GetMissingNeighbour(bool up, bool down, bool right, bool left)
    {
        List<Transform> neighbours = new List<Transform>();
        if (up) { neighbours.Add(topChecker); }
        if (down) { neighbours.Add(downChecker); }
        if (right) { neighbours.Add(rightChecker); }
        if (left) { neighbours.Add(leftChecker); }

        for (int i = 0; i < allSideCheckers.Length; i++)
        {
            if (!neighbours.Contains(allSideCheckers[i]))
            {
                return allSideCheckers[i];
            }
        }
        return null;
    }

    public string GetObjectType()
    {
        return activeObjectType.ToString();
    }

    private void LookAtNeighbour(Transform target)
    {
        Vector3 targetPostition = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(targetPostition);
    }

    private Transform GetSingularSide()
    {
        for (int i = 0; i < allSideCheckers.Length; i++)
        {
            RaycastHit[] hits = Physics.BoxCastAll(allSideCheckers[i].position, new Vector3(1, 1, 1) / 4, transform.up, Quaternion.identity, 1);

            for (int c = 0; c < hits.Length; c++)
            {
                if (hits[c].transform.tag == "Building")
                {
                    if (hits[c].transform != transform)
                    {
                        if (hits[c].transform.GetComponent<MeshTile>())
                        {
                            Debug.Log("Found meshtile");
                            if (hits[c].transform.GetComponent<MeshTile>().GetObjectType() == activeObjectType.ToString())
                            {
                                return hits[c].transform;
                            }
                        }
                    }
                }
            }
        }
        return null;
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
