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
    
    private bool _isActive = false;
    private bool _isInCooldown = false;
    private SteamAppearance _currentAppearance = SteamAppearance.Off;
    
    public bool IsActive() => _isActive;
    public float GetDamage() => damage;


    private void Start()
    {
        ChangeSteamState(SteamAppearance.Off);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (!_isActive && !_isInCooldown)
        {
            StartCoroutine(ActivateLaser());
        }
    }
    
    private IEnumerator ActivateLaser()
    {
        _isInCooldown = true;
        
        yield return new WaitForSeconds(activationDelay);
        _isActive = true;
        ChangeSteamState(SteamAppearance.On);
        
        yield return new WaitForSeconds(activeDuration);
        _isActive = false;
        ChangeSteamState(SteamAppearance.Off);
        
        _isInCooldown = false;
    }
    
    private void ChangeSteamState(SteamAppearance appearance)
    {
        _currentAppearance = appearance;
        onSteamImage?.SetActive(appearance == SteamAppearance.On);
    }
}
