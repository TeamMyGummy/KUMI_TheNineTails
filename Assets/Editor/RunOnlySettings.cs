#if UNITY_EDITOR
// RunOnlyToggleOnSettings.cs
using UnityEditor;
using UnityEngine;

public static class RunOnlySettings
{
    private const string PrefsKey = "RunOnly_IsEnabled";

    public static bool IsEnabled
    {
        get => EditorPrefs.GetBool(PrefsKey, true);
        set => EditorPrefs.SetBool(PrefsKey, value);
    }
}
#endif