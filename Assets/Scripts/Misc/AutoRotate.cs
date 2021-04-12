using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10;

    void Update()
    {
        transform.Rotate(transform.up * rotationSpeed * Time.deltaTime);
    }
}
