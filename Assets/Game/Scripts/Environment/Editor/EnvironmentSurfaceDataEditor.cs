using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnvironmentSurfaceData))]
public class EnvironmentSurfaceDataEditor : Editor
{
    private EnvironmentSurfaceData m_target;
    static bool m_showMappings = true;

    public void OnEnable()
    {
        m_target = (EnvironmentSurfaceData)target;
    }

    public override void OnInspectorGUI()
    {
        m_showMappings = EditorGUILayout.BeginFoldoutHeaderGroup(m_showMappings, "SurfaceData");

        if (!m_showMappings)
        {
            return;
        }

        for (var i = 0; i < m_target.SurfaceTypes.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            m_target.SurfaceTypes[i] = EditorGUILayout.TextField(m_target.SurfaceTypes[i]);
            m_target.SurfaceMovementModifiers[i] = EditorGUILayout.FloatField(m_target.SurfaceMovementModifiers[i]);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            m_target.SurfaceTypes.Add("");
            m_target.SurfaceMovementModifiers.Add(0.0f);
        }
        EditorGUILayout.EndHorizontal();
    }
}
