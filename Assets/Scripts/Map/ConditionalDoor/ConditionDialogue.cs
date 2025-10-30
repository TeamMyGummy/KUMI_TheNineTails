using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDialogue : MonoBehaviour
{
    [SerializeField] private string dialogueName;
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject prevPopup;
    [SerializeField] private bool isDialogue;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (prevPopup != null)
                prevPopup.SetActive(false);

            if (isDialogue)
            {
                Debug.Log("대화 시작!");
                YarnManager.Instance.RunDialogue(dialogueName, EnablePopup);
            }
            else
            {
                EnablePopup();
            }
        }
    }

    private void EnablePopup()
    {
        popup.SetActive(true);
        gameObject.SetActive(false);
    }
}
