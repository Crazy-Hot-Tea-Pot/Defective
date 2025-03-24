using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SecurityCameraController))]
public class SecurityCameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SecurityCameraController script = (SecurityCameraController)target;

        // Display a live preview of the Security Camera feed
        if (script.securityCamera && script.securityCamera.targetTexture != null)
        {
            GUILayout.Label("Camera Preview:", EditorStyles.boldLabel);
            Rect previewRect = GUILayoutUtility.GetRect(256, 144);
            EditorGUI.DrawPreviewTexture(previewRect, script.securityCamera.targetTexture);
        }

        DrawDefaultInspector();

        if (script.TriggerZone == null)
        {
            if (GUILayout.Button("Add Trigger Zone"))
            {
                if (script.TriggerZonePrefab != null)
                {
                    GameObject trigger = (GameObject)PrefabUtility.InstantiatePrefab(script.TriggerZonePrefab);
                    trigger.transform.SetParent(script.transform);
                    trigger.transform.localPosition = Vector3.zero;
                    trigger.transform.localRotation = Quaternion.identity;
                    SecurityCameraTrigger triggerScript = trigger.GetComponent<SecurityCameraTrigger>();
                    triggerScript.controller = script;
                    script.TriggerZone = trigger;
                    Undo.RegisterCreatedObjectUndo(trigger, "Add Trigger Zone");
                    EditorUtility.SetDirty(script);
                }
                else
                {
                    Debug.LogError("Trigger Zone Prefab is not assigned in SecurityCameraController!");
                }
            }
        }
        else if(script.TriggerZone != null)
        {
            GUILayout.Space(10);
            GUILayout.Label("Trigger Zone Settings", EditorStyles.boldLabel);
            SerializedProperty triggerZonePosition = serializedObject.FindProperty("triggerZonePosition");
            SerializedProperty triggerZoneRotation = serializedObject.FindProperty("triggerZoneRotation");
            SerializedProperty triggerZoneSize = serializedObject.FindProperty("triggerZoneSize");

            // Draw fields properly so Unity tracks changes
            EditorGUILayout.PropertyField(triggerZonePosition, new GUIContent("Position"));
            EditorGUILayout.PropertyField(triggerZoneRotation, new GUIContent("Rotation"));
            EditorGUILayout.PropertyField(triggerZoneSize, new GUIContent("Size"));

            // Apply the changes
            serializedObject.ApplyModifiedProperties();


            //if (script.TriggerZone != null)
            //{
            //    script.TriggerZone.transform.localPosition = script.triggerZonePosition;
            //    script.TriggerZone.transform.localEulerAngles = script.TriggerZoneRotation;
            //    script.TriggerZone.transform.localScale = script.TriggerZoneSize;
            //}
            //BoxCollider collider = script.TriggerZone.GetComponent<BoxCollider>();
            //if (collider != null)
            //{
            //    Vector3 size = collider.size;
            //    size = EditorGUILayout.Vector3Field("Size", size);
            //    collider.size = size;
            //}
        }
    }
}
