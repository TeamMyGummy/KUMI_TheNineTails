using UnityEngine;
using GameAbilitySystem;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject uiSkillPrefab; 
    [SerializeField] private Transform canvas;

    private UI_Skill uiSkillInstance;
    private AbilitySystem abilitySystem;
    private bool hasTabOpenedUI = false;

    void Start()
    {
        abilitySystem = new AbilitySystem();
        abilitySystem.Init("Domain/Player");
        abilitySystem.GrantAllAbilities();
        abilitySystem.SetSceneState(player);
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            hasTabOpenedUI = true;
            ToggleSkillUI();
        }
        else if (hasTabOpenedUI && Keyboard.current.qKey.wasPressedThisFrame)
        {
            uiSkillInstance.OnClickSkillBtn();
        }
        else if (hasTabOpenedUI && Keyboard.current.eKey.wasPressedThisFrame)
        {
            uiSkillInstance.OnClickJournalBtn();
        }
    }

    void ToggleSkillUI()
    {
        if (uiSkillInstance == null)
        {
            var go = Instantiate(uiSkillPrefab, canvas);
            go.SetActive(true); 
            uiSkillInstance = go.GetComponent<UI_Skill>();
            uiSkillInstance.SetAbilitySystem(abilitySystem);
            uiSkillInstance.ForceReferenceReconnect();

            player.GetComponent<PlayerInput>().enabled = false;

            return;
        }

        bool willClose = uiSkillInstance.gameObject.activeSelf;
        uiSkillInstance.TogglePopupExternally();

        player.GetComponent<PlayerInput>().enabled = willClose;
    }
}
