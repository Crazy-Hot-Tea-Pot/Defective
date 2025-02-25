using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [System.Serializable]
    public class CursorData
    {
        // Layer index (dropdown in inspector)
        [Tooltip("Select the Layer for this cursor mapping")]
        public int layerIndex;

        // Cursor image
        [Tooltip("Image of the cursor")]
        public Texture2D cursorTexture;

        // Cursor hotspot
        [Tooltip("Hotspot of the cursor")]
        public Vector2 hotspot;
    }

    // Global setting to determine if Unity's default cursor is used
    [Tooltip("If true, Unity's default cursor will be used for the entire game.")]
    public bool useUnityDefaultCursor = true;

    // List of cursor mappings based on layer
    public List<CursorData> cursorMappings;

    // Custom default cursor (used when global flag is false)
    public Texture2D defaultCursor;
    public Vector2 defaultHotspot;

    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnSceneChange += SceneChange;
        mainCamera = Camera.main;
        SetCursorBasedOnDefault();
    }

    // Update is called once per frame
    void Update()
    {
        if (!useUnityDefaultCursor && mainCamera != null)
            HandleCursor();
    }

    // Handles the cursor changes based on what the mouse is hovering over
    private void HandleCursor()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            bool cursorSet = false;

            foreach (var mapping in cursorMappings)
            {
                if (hit.collider.gameObject.layer == mapping.layerIndex)
                {
                    SetCursor(mapping.cursorTexture, mapping.hotspot);
                    cursorSet = true;
                    break;
                }
            }

            if (!cursorSet)
            {
                SetCursorBasedOnDefault();
            }
        }
        else
        {
            SetCursorBasedOnDefault();
        }
    }

    // Sets default cursor based on global settings
    private void SetCursorBasedOnDefault()
    {
        if (useUnityDefaultCursor)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(defaultCursor, defaultHotspot, CursorMode.Auto);
        }
    }

    // Sets the cursor with specified texture and hotspot
    private void SetCursor(Texture2D texture, Vector2 hotspot)
    {
        Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
    }

    private void SceneChange(Levels newLevel)
    {
        switch (newLevel)
        {
            case Levels.Title:
            case Levels.Loading:
            case Levels.Settings:
            case Levels.Credits:
                mainCamera = null;
                break;
            default:
                mainCamera = Camera.main;
                SetCursorBasedOnDefault();
                break;
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.OnSceneChange -= SceneChange;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CursorManager), true)]
    public class CursorManagerEditor : Editor
    {
        private string[] availableLayers;
        private int[] availableLayerIndices;

        private void OnEnable()
        {
            availableLayers = InternalEditorUtility.layers;
            availableLayerIndices = GetLayerIndices();
        }

        private int[] GetLayerIndices()
        {
            int[] indices = new int[availableLayers.Length];
            for (int i = 0; i < availableLayers.Length; i++)
            {
                indices[i] = LayerMask.NameToLayer(availableLayers[i]);
            }
            return indices;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("useUnityDefaultCursor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultCursor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultHotspot"));

            SerializedProperty mappings = serializedObject.FindProperty("cursorMappings");
            EditorGUILayout.LabelField("Cursor Mappings", EditorStyles.boldLabel);

            // Add/Remove buttons for cursorMappings
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Mapping"))
            {
                mappings.arraySize++;
            }
            if (GUILayout.Button("Remove Last Mapping") && mappings.arraySize > 0)
            {
                mappings.DeleteArrayElementAtIndex(mappings.arraySize - 1);
            }
            EditorGUILayout.EndHorizontal();

            // Custom rendering of each element
            if (mappings.isArray)
            {
                for (int i = 0; i < mappings.arraySize; i++)
                {
                    SerializedProperty element = mappings.GetArrayElementAtIndex(i);
                    SerializedProperty layerProp = element.FindPropertyRelative("layerIndex");

                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.PropertyField(element.FindPropertyRelative("cursorTexture"));
                    EditorGUILayout.PropertyField(element.FindPropertyRelative("hotspot"));

                    // Dropdown for layers (showing names but storing index)
                    int selectedIndex = System.Array.IndexOf(availableLayerIndices, layerProp.intValue);
                    if (selectedIndex < 0) selectedIndex = 0;

                    selectedIndex = EditorGUILayout.Popup("Layer", selectedIndex, availableLayers);
                    layerProp.intValue = availableLayerIndices[selectedIndex];

                    EditorGUILayout.EndVertical();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
