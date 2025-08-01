#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public static class AutoDeleteSaveData
{
    static AutoDeleteSaveData()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        // Play 모드가 끝날 때
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            string path = Util.JsonLoader.GetDynamicDataPath("gamedata_0");
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log("[AutoDeleteSaveData] gamedata_0.json 삭제됨");
            }
        }
    }
}
#endif
