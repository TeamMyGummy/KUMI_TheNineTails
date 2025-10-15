using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDialogue : MonoBehaviour
{
    [SerializeField] private string dialogueName;
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject prevPopup;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (prevPopup != null)
                prevPopup.SetActive(false);
            
            YarnManager.Instance.RunDialogue(dialogueName, EnablePopup);
            
            gameObject.SetActive(false);
        }
    }

    private void EnablePopup()
    {
        popup.SetActive(true);
    }
}
