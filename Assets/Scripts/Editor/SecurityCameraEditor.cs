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
        else
        {
            GUILayout.Label("Trigger Zone Settings", EditorStyles.boldLabel);
            script.TriggerZone.transform.localPosition = EditorGUILayout.Vector3Field("Position", script.TriggerZone.transform.localPosition);
            script.TriggerZone.transform.localEulerAngles = EditorGUILayout.Vector3Field("Rotation", script.TriggerZone.transform.localEulerAngles);
            script.TriggerZone.transform.localScale = EditorGUILayout.Vector3Field("Size", script.TriggerZone.transform.localScale);
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
