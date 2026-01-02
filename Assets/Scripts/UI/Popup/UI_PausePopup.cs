using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 이 스크립트는 PausePopup 프리팹 자체에 붙여서 버튼들을 연결하는 용도입니다.
public class UI_PausePopup : MonoBehaviour
{
    [Header("Buttons (Order: Resume, Settings, Main)")]
    public Button[] menuButtons;

    [Header("Arrows (Order: Resume, Settings, Main)")]
    public GameObject[] menuArrows;
}