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
            ToggleSkillUI();
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
