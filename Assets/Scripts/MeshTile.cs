using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTile : MonoBehaviour
{
    private enum objectType { Wall, Road }
    [SerializeField] private bool ignoreCurrentMeshChange = false;
    [SerializeField] private objectType activeObjectType;
    [SerializeField] private Transform topChecker, downChecker, leftChecker, rightChecker, meshObject;
    [SerializeField] private Transform[] allSideCheckers;
    [SerializeField] private GameObject fourSidesObject, threeSidesObject, CornerObject, twoSidesObject, oneSideObject, zeroSideObject;
    private bool canBeChecked = true;

    private IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(1);
        canBeChecked = true;
    }

    private void Start()
    {
        CheckNeighbours();
    }

    public void CheckNeighbours()
    {
        if (!canBeChecked || ignoreCurrentMeshChange)
        {
            return;
        }

        canBeChecked = false;

        DisableAllWalls();

        bool topActive = CheckIfSideHasNeighbour(topChecker);
        bool downActive = CheckIfSideHasNeighbour(downChecker);
        bool leftActive = CheckIfSideHasNeighbour(leftChecker);
        bool rightActive = CheckIfSideHasNeighbour(rightChecker);

        int connectedSides = 0;

        if (topActive) { connectedSides += 1; }
        if (downActive) { connectedSides += 1; }
        if (leftActive) { connectedSides += 1; }
        if (rightActive) { connectedSides += 1; }

        StartCoroutine(StartCooldown());

        switch (connectedSides)
        {
            case 4:
                fourSidesObject.SetActive(true);
                break;
            case 3:
                threeSidesObject.SetActive(true);
                LookAtNeighbour(GetMissingNeighbour(topActive, downActive, rightActive, leftActive).position, transform);
                break;
            case 2:
                if (topActive && downActive) { twoSidesObject.SetActive(true); LookAtNeighbour(topChecker.position, transform); break; }
                if (leftActive && rightActive) { twoSidesObject.SetActive(true); LookAtNeighbour(leftChecker.position, transform); break; }

                CornerObject.SetActive(true);
                LookAtNeighbour(GetMiddlePointForCorner(topActive, downActive, rightActive, leftActive), transform);
                break;
            case 1:
                oneSideObject.SetActive(true);
                LookAtNeighbour(GetSingularSide().position, transform);
                break;
            case 0:
                zeroSideObject.SetActive(true);
                break;
        }
    }

    private Vector3 GetMiddlePointForCorner(bool up, bool down, bool right, bool left)
    {
        List<Transform> neighbours = new List<Transform>();
        if (up) { neighbours.Add(topChecker); }
        if (down) { neighbours.Add(downChecker); }
        if (right) { neighbours.Add(rightChecker); }
        if (left) { neighbours.Add(leftChecker); }


        Vector3 cornerPos = neighbours[0].position + (neighbours[1].position - neighbours[0].position) / 2;
        return cornerPos;
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

    private void LookAtNeighbour(Vector3 target, Transform objectToMove)
    {
        Vector3 targetPostition = new Vector3(target.x, transform.position.y, target.z);
        objectToMove.LookAt(targetPostition);
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
        RaycastHit[] hits = Physics.BoxCastAll(sideChecker.position, new Vector3(1, 1, 1) / 8, transform.up, Quaternion.identity, 1);

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
                            if (hits[i].transform.GetComponent<MeshTile>().isActiveAndEnabled)
                            {
                                hits[i].transform.GetComponent<MeshTile>().CheckNeighbours();
                            }
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    private void DisableAllWalls()
    {
        oneSideObject.SetActive(false);
        twoSidesObject.SetActive(false);
        threeSidesObject.SetActive(false);
        fourSidesObject.SetActive(false);
        zeroSideObject.SetActive(false);
        CornerObject.SetActive(false);
        transform.rotation = new Quaternion(0,0,0,0);
    }
}
