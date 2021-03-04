using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomText : MonoBehaviour
{
    [SerializeField] private string[] possibleTexts;

    private void Start()
    {
        GetComponent<TMPro.TextMeshProUGUI>().text = possibleTexts[Random.Range(0, possibleTexts.Length)];
        Destroy(this);
    }
}
