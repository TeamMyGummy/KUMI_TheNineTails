using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierPath))]
public class BezierPathEditor : Editor
{
    private BezierPath path;
    
    // 이 값들을 조절해서 원하는 크기로 맞추세요!
    private const float handleSize = 0.3f; // 점(핸들) 크기
    private const float curveThickness = 4f; // 곡선 굵기

    private void OnSceneGUI()
    {
        path = (BezierPath)target;

        // 경로의 모든 점을 핸들로 표시하고 수정 가능하게 만듭니다.
        for (int i = 0; i < path.PointCount; i++)
        {
            // 앵커는 청록, 컨트롤은 노랑으로 색상 구분
            Handles.color = (i % 3 == 0) ? Color.cyan : Color.yellow; 
            
            Vector3 currentPoint = path[i];
            
            // 핸들 크기를 handleSize 변수로 조절합니다.
            var fmh_26_67_638947702879657845 = Quaternion.identity; Vector3 newPos = Handles.FreeMoveHandle(currentPoint, handleSize, Vector3.zero, Handles.SphereHandleCap);
            
            if (currentPoint != newPos)
            {
                Undo.RecordObject(path, "Move Point");
                path[i] = newPos;
            }
        }
        
        // 점과 점 사이를 선으로 연결해서 보여줍니다.
        for (int i = 0; i < path.PointCount -1; i++)
        {
            if (i % 3 != 0)
            {
                Handles.color = Color.gray;
                Handles.DrawLine(path[i], path[i+1]);
            }
        }

        // 베지어 곡선을 그립니다.
        for (int i = 0; i < path.SegmentCount; i++)
        {
            Vector3[] points = path.GetPointsInSegment(i);
            
            // 곡선 굵기를 curveThickness 변수로 조절합니다.
            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, curveThickness);
        }
    }
    
    // 인스펙터 창에 '세그먼트 추가' 버튼을 만듭니다.
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Add Segment"))
        {
            path = (BezierPath)target;
            Undo.RecordObject(path, "Add Segment");
            path.AddSegment(path[path.PointCount - 1] + Vector3.right * 2f);
            EditorUtility.SetDirty(path);
        }
    }
}