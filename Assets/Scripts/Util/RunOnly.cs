// RunOnly.cs (Editor 폴더 밖에 위치)

public static class RunOnly
{
#if UNITY_EDITOR
    // 에디터 스크립트가 이 변수에 접근해서 값을 바꿔줄 것입니다.
    public static bool IsEditorToggleEnabled = true;
#endif

    public static bool ShouldRun()
    {
#if UNITY_EDITOR
        // 이제 Editor 폴더의 클래스를 직접 참조하지 않고,
        // 값을 밀어넣어 준 이 변수를 사용합니다.
        return IsEditorToggleEnabled;
#else
        // 빌드에서는 항상 true
        return true;
#endif
    }
}