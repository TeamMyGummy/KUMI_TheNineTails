#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabComment))]
public class PrefabCommentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var comment = (PrefabComment)target;

        EditorGUILayout.HelpBox(comment.comment, MessageType.Info);

        EditorGUI.BeginChangeCheck();
        string newComment = EditorGUILayout.TextArea(comment.comment, GUILayout.Height(30));
        if (EditorGUI.EndChangeCheck())
        {
            comment.comment = newComment;
            EditorUtility.SetDirty(comment);
        }
    }
}
#endif