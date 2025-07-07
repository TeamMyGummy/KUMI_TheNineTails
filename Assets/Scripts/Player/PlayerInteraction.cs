using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private Lantern currentLantern;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Lantern lantern))
            currentLantern = lantern;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Lantern lantern) && currentLantern == lantern)
            currentLantern = null;
    }


    void Update()
    {
        if (currentLantern != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentLantern.Interact();
        }
    }
}
