using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burnable : MonoBehaviour
{
    [Tooltip("Building health is in seconds")]
    [SerializeField] private float buildingHealth = 300;
    private float currentHealth;
    [SerializeField] private ParticleSystem fireParticles;

    private void Start()
    {
        currentHealth = buildingHealth;
    }

    public void ToggleFire(bool on)
    {
        enabled = on;
        if (on) { fireParticles.Play(); } else { fireParticles.Stop(); currentHealth = buildingHealth; }
    }

    private void Update()
    {
        currentHealth -= 1 * Time.deltaTime;

        if(currentHealth <= 0)
        {

        }
    }
}
