// 파일 위치: Assets/Editor/StripPrefabComments.cs
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class StripPrefabComments : IProcessSceneWithReport
{
    public int callbackOrder => 0;

    public void OnProcessScene(UnityEngine.SceneManagement.Scene scene, BuildReport report)
    {
        foreach (var comment in Object.FindObjectsOfType<PrefabComment>())
        {
            Object.DestroyImmediate(comment);  // 컴포넌트만 제거
        }
    }
}