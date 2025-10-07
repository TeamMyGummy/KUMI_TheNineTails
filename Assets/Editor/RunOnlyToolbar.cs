#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[InitializeOnLoad]
public static class RunOnlyToolbar
{
    private static ScriptableObject toolbar;
    private static VisualElement root;

    static RunOnlyToolbar()
    {
        EditorApplication.delayCall += () =>
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
            SetToggleState(RunOnlySettings.IsEnabled);
        };
    }

    private static void Update()
    {
        if (toolbar == null)
        {
            var editorAssembly = typeof(Editor).Assembly;
            var toolbarType = editorAssembly.GetType("UnityEditor.Toolbar");
            var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
            
            if (toolbars.Length > 0)
            {
                toolbar = (ScriptableObject)toolbars[0];
                
                var rootField = toolbarType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                if (rootField != null)
                {
                    root = rootField.GetValue(toolbar) as VisualElement;
                    if (root != null)
                    {
                        CreateToggle();
                        EditorApplication.update -= Update;
                    }
                }
            }
        }
    }

    private static void CreateToggle()
    {
        // Play 버튼이 있는 영역 찾기
        var playModeButtons = root.Q("ToolbarZonePlayMode");
        if (playModeButtons == null) return;

        // 이미 추가되었는지 확인
        if (root.Q("RunOnlyToggleOn") != null) return;

        // 토글 컨테이너 생성
        var container = new VisualElement();
        container.name = "RunOnly";
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;
        container.style.marginRight = 8;
        container.style.marginLeft = 4;

        // 토글 생성
        var toggle = new Toggle
        {
            value = RunOnlySettings.IsEnabled,
            tooltip = "RunOnly 활성화/비활성화\n이 토글이 켜져있을 때만 [RunOnly] 속성이 붙은 함수가 실행됩니다."
        };
        toggle.style.marginRight = 2;

        toggle.RegisterValueChangedCallback(evt =>
        {
            SetToggleState(evt.newValue);
        });

        // 레이블 생성
        var label = new Label("Run");
        label.style.unityTextAlign = TextAnchor.MiddleCenter;
        label.style.fontSize = 9;
        label.style.marginTop = 1;

        container.Add(toggle);
        container.Add(label);

        // Play 버튼 앞에 삽입
        playModeButtons.Insert(0, container);
    }
    
    private static void SetToggleState(bool isEnabled)
    {
        // 1. EditorPrefs에 상태 저장 (기존 로직)
        RunOnlySettings.IsEnabled = isEnabled;

        // 2. 런타임 스크립트의 static 변수에 현재 상태를 '밀어넣어' 줍니다.
        RunOnly.IsEditorToggleEnabled = isEnabled;

        Debug.Log($"RunOnlyToggleOn: {(isEnabled ? "활성화" : "비활성화")}");
    }
}
#endif 