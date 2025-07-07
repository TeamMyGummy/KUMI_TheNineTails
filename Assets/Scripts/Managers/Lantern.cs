using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lantern : MonoBehaviour
{

    public LanternState State { get; private set; } = LanternState.Off;
    public GameObject smallFlameEffect;
    public GameObject bigFlameEffect;
    public GameObject interactionUI;

    void Start()
    {
        UpdateVisuals();
        interactionUI.SetActive(false);
    }


    public void Interact()
    {
        if (State == LanternState.Off)
        {
            SetState(LanternState.BigFlame);
            LanternManager.Instance.SetActiveLantern(this);
        }
        else
        {
            Save();
        }
    }

    public void SetState(LanternState newState)
    {
        State = newState;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        smallFlameEffect.SetActive(State == LanternState.SmallFlame);
        bigFlameEffect.SetActive(State == LanternState.BigFlame);
    }

    private void Save()
    {
        // 저장은 어케하지
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            interactionUI.SetActive(false);
    }

}
