using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private Animator animator;
    private bool trapActivated = false;
    [SerializeField] private int destroyTimer;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy" && !trapActivated)
        {
            trapActivated = true;
            animator.Play("ActivateTrap");
            Destroy(gameObject, destroyTimer);
        }
    }
}
