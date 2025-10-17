using UnityEngine;
using TMPro;

public class LoadGamePopup : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI floorText;
    [SerializeField]
    private TextMeshProUGUI sectionText;

    public void SetInfo(string floor, string section)
    {
        if (floorText != null)
            floorText.text = floor;
        
        if (sectionText != null)
            sectionText.text = section;
    }
}