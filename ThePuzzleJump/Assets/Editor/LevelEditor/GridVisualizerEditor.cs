using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridVisualizer))]
public class GridVisualizerEditor : Editor
{
    private void OnSceneGUI()
    {
        GridVisualizer visualizer = (GridVisualizer)target;

        EditorGUI.BeginChangeCheck();

        Vector3 newA = Handles.PositionHandle(
            visualizer.EditorPointA,
            Quaternion.identity
        );

        Vector3 newB = Handles.PositionHandle(
            visualizer.EditorPointB,
            Quaternion.identity
        );

        Handles.Label(newA + Vector3.up * 0.2f, "Point A");
        Handles.Label(newB + Vector3.up * 0.2f, "Point B");

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(visualizer, "Move Grid Area Points");
            visualizer.EditorPointA = newA;
            visualizer.EditorPointB = newB;
            EditorUtility.SetDirty(visualizer);
        }
    }
}
