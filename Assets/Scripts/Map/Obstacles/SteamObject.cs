using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SteamAppearance
{
    Off,
    On,
}

public class SteamObject : MonoBehaviour
{
    [SerializeField] private GameObject onSteamImage; 
    
    [SerializeField] private float damage = 1.0f;
    [SerializeField] private float activationDelay = 0.5f;
    [SerializeField] private float activeDuration = 6.0f;
    
    private bool isActive = false;
    private bool isInCooldown = false;
    private SteamAppearance currentAppearance = SteamAppearance.Off;
    
    public bool IsActive() => isActive;
    public float GetDamage() => damage;


    private void Start()
    {
        ChangeSteamState(SteamAppearance.Off);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (!isActive && !isInCooldown)
        {
            StartCoroutine(ActivateLaser());
        }
    }
    
    private IEnumerator ActivateLaser()
    {
        isInCooldown = true;
        
        yield return new WaitForSeconds(activationDelay);
        isActive = true;
        ChangeSteamState(SteamAppearance.On);
        
        yield return new WaitForSeconds(activeDuration);
        isActive = false;
        ChangeSteamState(SteamAppearance.Off);
        
        isInCooldown = false;
    }
    
    public void ChangeSteamState(SteamAppearance appearance)
    {
        currentAppearance = appearance;
        onSteamImage?.SetActive(appearance == SteamAppearance.On);
    }
}
