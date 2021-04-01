using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burnable : MonoBehaviour
{
    [SerializeField] private float buildingHealth = 300;
    private float currentHealth, deactivateTimer;
    [SerializeField] private ParticleSystem fireParticles;

    private void Start()
    {
        currentHealth = buildingHealth;
        deactivateTimer = buildingHealth * 2;
    }

    public void ToggleFire(bool on)
    {
        enabled = on;
        if (on) { fireParticles.Play(); } else { fireParticles.Stop(); currentHealth = buildingHealth; deactivateTimer = buildingHealth * 2; }
    }

    public bool IsBurning()
    {
        return fireParticles.isPlaying;
    }

    private void Update()
    {
        if (fireParticles.isPlaying)
        {
            currentHealth -= 1 * Time.deltaTime;

            if (currentHealth <= 0)
            {
                Debug.Log("Building has burned down");
                GameManager.RemoveBuildingFromSave(GetComponent<ObjectID>().GetID());
                Destroy(gameObject);
            }

            deactivateTimer -= 1 * Time.deltaTime;

            if(deactivateTimer <= 0)
            {
                ToggleFire(false);
            }
        }
    }
}
