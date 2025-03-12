using System.Collections;
using System.Collections.Generic;
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
    }

    // List of cursor mappings based on layer
    public List<CursorData> cursorMappings;

    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnSceneChange += SceneChange;
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera != null)
            HandleCursor();
    }

    // Handles the cursor changes based on what the mouse is hovering over
    private void HandleCursor()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            foreach (var mapping in cursorMappings)
            {
                if (hit.collider.gameObject.layer == mapping.layerIndex)
                {
                    SetCursor(mapping.cursorTexture);
                    return;
                }
            }
        }

        // If nothing was hit or no matching layer was found, reset to default cursor
        ResetCursor();
    }
    private void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    private void SetCursor(Texture2D texture)
    {
        Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
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
                break;
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.OnSceneChange -= SceneChange;
    }
}