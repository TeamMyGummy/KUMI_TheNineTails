using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lantern : MonoBehaviour
{
    private Lantern currentLantern;

    public LanternState State { get; private set; } = LanternState.Off;
    [SerializeField] private GameObject offFlameEffect;
    [SerializeField] private GameObject smallFlameEffect;
    [SerializeField] private GameObject bigFlameEffect;
    [SerializeField] private GameObject interactionUI;

    void Start()
    {
        UpdateVisuals();
        interactionUI.SetActive(false);
    }

    void Update()
    {
        //if (currentLantern != null && Keyboard.current.eKey.wasPressedThisFrame)
        //{
        //    currentLantern.Interact();
        //}

        if (currentLantern != null && Input.GetKeyDown(KeyCode.E))
        {
            currentLantern.InteractWithLantern();
        }
    }


    public void InteractWithLantern()
    {
        if (State == LanternState.Off)
        {
            SetState(LanternState.BigFlame);
            //LanternManager.Instance.SetBigFlame(this);
        }
        else
        {
            //Save();
        }
    }

    public void SetState(LanternState newState)
    {
        State = newState;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        offFlameEffect.SetActive(State == LanternState.Off);
        smallFlameEffect.SetActive(State == LanternState.SmallFlame);
        bigFlameEffect.SetActive(State == LanternState.BigFlame);
    }

    //private void Save()
    //{

    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            interactionUI.SetActive(true);

        //if (other.TryGetComponent(out Lantern lantern))
        //    currentLantern = lantern;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            interactionUI.SetActive(false);
        //if (other.TryGetComponent(out Lantern lantern) && currentLantern == lantern)
        //    currentLantern = null;
    }


}
