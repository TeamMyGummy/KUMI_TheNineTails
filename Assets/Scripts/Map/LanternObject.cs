using System;
using UnityEngine;

public enum LanternAppearance
{
    Off,
    Small,
    Big,
}

public enum LanternState
{
    Off,
    SmallFlame,
    BigFlame,
}

//LanternObject가 존재하는 씬에는 반드시 Lantern이 씬에 존재해야 함
public class LanternObject : MonoBehaviour
{
    public int LanternKey { get; private set; }

    [SerializeField] private GameObject offLanternImage;      
    [SerializeField] private GameObject smallLanternImage;   
    [SerializeField] private GameObject bigLanternImage;    

    [SerializeField] private GameObject interactionUI;

    public bool isInteracting = false;
    private bool playerInRange = false;
    private LanternAppearance currentAppearance = LanternAppearance.Off;

    public Action<int> Interacted;

    void Start()
    {
        ChangeLanternState(LanternAppearance.Off);

        if (interactionUI != null)
            interactionUI.SetActive(false);

        LanternKey = transform.GetSiblingIndex();

    }

    public void Bind(Action<int> interacted)
    {
        Interacted = interacted;
    }

    public void ChangeLanternState(LanternAppearance appearance)
    {
        currentAppearance = appearance;

        if (offLanternImage != null) offLanternImage.SetActive(false);
        if (smallLanternImage != null) smallLanternImage.SetActive(false);
        if (bigLanternImage != null) bigLanternImage.SetActive(false);

        switch (appearance)
        {
            case LanternAppearance.Off:
                if (offLanternImage != null)
                    offLanternImage.SetActive(true);
                break;

            case LanternAppearance.Small:
                if (smallLanternImage != null)
                    smallLanternImage.SetActive(true);
                break;

            case LanternAppearance.Big:
                if (bigLanternImage != null)
                    bigLanternImage.SetActive(true);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactionUI != null)
                interactionUI.SetActive(true);

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetLanternObject(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactionUI != null)
                interactionUI.SetActive(false);

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetLanternObject(null); // 더 이상 상호작용 대상 아님
            }
        }
    }

    public void InteractWithLantern()
    {
        if (playerInRange)
        {
            Interacted?.Invoke(LanternKey);
        }
    }

    //랜턴 키 설정(외부에서 호출 가능)
    public void SetLanternKey(int key)
    {
        LanternKey = key;
    }

}


