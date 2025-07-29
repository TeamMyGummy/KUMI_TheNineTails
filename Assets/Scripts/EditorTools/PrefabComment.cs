using UnityEngine;

[DisallowMultipleComponent]
public class PrefabComment : MonoBehaviour
{
    [TextArea(0, 300)]
    public string comment;
}