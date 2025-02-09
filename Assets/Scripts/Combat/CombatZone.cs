using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatZone : MonoBehaviour
{
    [System.Serializable]
    public class EnemyPlacementData
    {
        public GameObject enemyObject;
        public EnemyManager.EnemyType enemyType;

        public EnemyPlacementData(GameObject obj, EnemyManager.EnemyType type)
        {
            enemyObject = obj;
            enemyType = type;
        }
    }
    public bool CombatZoneSet;

    public List<EnemyPlacementData> EnemiesInZone = new List<EnemyPlacementData>();

    public GameObject EnemyPlaceholder;

    [Header("Color Status")]
    public Color SavedColor;

    [Range(0f, 1f)]
    public float savedTransparicy;

    public Color NotSavedColor;

    [Range(0f, 1f)]
    public float NotSaveTransparicy;

    public Color InCombatColor;

    [Range(0f, 1f)]
    public float CombatTransparicy;

    [Header("Combat Area Size")]
    public Vector3 areaSize = new Vector3(1f, 1f, 1f);

    [Header("Player Position (XZ editable, Y auto-adjusts to bottom of CombatArea)")]
    public Vector2 playerPositionXZOffset = Vector2.zero;

    [Header("Camera Position & Rotation.")]
    public Vector3 cameraPositionOffset = Vector3.zero;

    [Header("Camera target for it to look at.")]
    public Vector3 cameraTargetPositionOffset = Vector3.zero;

    [Header("Needed objects No Need to Change of these.")]
    public GameObject CombatArea;
    public BoxCollider zoneCollider;
    public LayerMask enemyLayer;
    public Material combatMaterial;
    public Renderer CombatAreaRender;
    public GameObject PlayerPosition;
    public GameObject CombatCameraPosition; 
    public GameObject CameraTarget;

    private Transform playerPosition;
    private CameraController MainCamera;
    private PlayerController Player;
    private CombatController CombatController;

    void OnValidate()
    {
        // Update the size of the 3D object and BoxCollider when size changes in the Inspector
        if (CombatArea != null)
        {
            // Update the Transform scale for the visual size
            CombatArea.transform.localScale = areaSize;

            // Update the BoxCollider size to match the Transform scale
            if (zoneCollider != null)
            {
                zoneCollider.size = Vector3.one; // Reset to default size (relative to scale)
            }
        }

        // Update PlayerPosition
        if (PlayerPosition != null)
        {
            Vector3 combatAreaBottom = CombatArea.transform.position - new Vector3(0, CombatArea.transform.localScale.y / 2, 0);
            PlayerPosition.transform.position = new Vector3(
                combatAreaBottom.x + playerPositionXZOffset.x,
                combatAreaBottom.y + 0.5f,
                combatAreaBottom.z + playerPositionXZOffset.y
            );
        }

        // Update CombatCameraPosition
        if (CombatCameraPosition != null)
        {
            CombatCameraPosition.transform.position = transform.position + cameraPositionOffset;
        }

        if (CameraTarget != null)
        {
            CameraTarget.transform.position = transform.position + cameraTargetPositionOffset;
        }

        // Ensure Enemy are inside the Combat Zone
        ValidateEnemyPositions();
    }


    // Start is called before the first frame update
    void Start()
    {
        // Ensure unique material instance
        combatMaterial = CombatAreaRender.material = new Material(CombatAreaRender.material);

        combatMaterial.SetFloat("_Transparicy", 0f);

        // Save positions
        playerPosition = PlayerPosition.transform;

        // Deactivate positions
        PlayerPosition.SetActive(false);

        // **Disable enemy placeholders at runtime**
        foreach (var enemyData in EnemiesInZone)
        {
            if (enemyData.enemyObject != null)
            {
                // Hide placeholders
                enemyData.enemyObject.SetActive(false);
            }
        }

        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        CombatController = GameObject.FindGameObjectWithTag("CombatController").GetComponent<CombatController>();
    }
    
    public void PlayerEnteredCombatZone()
    {
        Debug.Log("Player entered the combat zone.");
        

        combatMaterial = CombatAreaRender.material = new Material(CombatAreaRender.material);

        combatMaterial.SetColor("_Base_Color",InCombatColor);

       // Get the bounds of the collider
       Bounds zoneBounds = zoneCollider.bounds;

        // Use Physics.OverlapBox to find all colliders in the zone
        Collider[] colliders = Physics.OverlapBox(
            zoneBounds.center,
            zoneBounds.extents,
            Quaternion.identity,
            enemyLayer
        );

        // Iterate through the detected colliders
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                // Add the enemy to the EnemyManager's combat list
                EnemyManager.Instance.AddCombatEnemy(collider.gameObject);
                Debug.Log("Added enemy to combat list: " + collider.name);
            }
        }

        Player.MovePlayerToPosition(playerPosition.position);


        MainCamera.UpdateCombatCamera(CombatCameraPosition);


        CombatController.StartCombat(this.gameObject);

        CombatController.CombatArea=CombatArea;
    }
    /// <summary>
    /// Sav combat zone settings
    /// </summary>
    public void SaveCombatZone()
    {
        if (EnemiesInZone.Count == 0)
        {
            Debug.LogError("Cannot save CombatZone without at least one enemy.");
            return;
        }

        // Ensure unique material instance
        combatMaterial = CombatAreaRender.material = new Material(CombatAreaRender.material);

        if (CombatZoneSet)
        {
            Debug.LogError("CombatZone already Set.");
            return;
        }

        Bounds combatAreaBounds = zoneCollider.bounds;

        // Check if PlayerPosition is inside the CombatArea
        if (!combatAreaBounds.Contains(PlayerPosition.transform.position))
        {
            Debug.LogError("PlayerPosition is outside the CombatArea. Please adjust its position.");
            return;
        }

        // Check if CombatCameraPosition is inside the CombatArea
        if (!combatAreaBounds.Contains(CombatCameraPosition.transform.position))
        {
            Debug.LogError("CameraPosition is outside the CombatArea. Please adjust its position.");
            return;
        }

        // Set material properties
        combatMaterial.SetColor("_Base_Color", SavedColor);
        combatMaterial.SetFloat("_Transparicy", savedTransparicy);       

        CombatZoneSet = true;
        Debug.Log("Combat Zone saved successfully.");
    }

    /// <summary>
    /// Allow CombatZone to be edited
    /// </summary>
    public void EditCombatZone()
    {
        // Ensure unique material instance
        combatMaterial = CombatAreaRender.material = new Material(CombatAreaRender.material);

        combatMaterial.SetColor("_Base_Color", NotSavedColor);
        combatMaterial.SetFloat("_Transparicy", NotSaveTransparicy);
        CombatZoneSet = false;
    }

    /// <summary>
    /// Get data from combat zone to spawn enemies
    /// </summary>
    /// <returns></returns>
    public List<(Vector3 position, EnemyManager.EnemyType type)> GetEnemySpawnData()
    {
        List<(Vector3, EnemyManager.EnemyType)> spawnData = new();

        foreach (var enemyData in EnemiesInZone)
        {
            if (enemyData.enemyObject != null)
            {
                // Convert local position to world position
                Vector3 worldPosition = transform.TransformPoint(enemyData.enemyObject.transform.localPosition);
                spawnData.Add((worldPosition, enemyData.enemyType));
            }
        }

        return spawnData;
    }


    /// <summary>
    /// Handles position of enemies in the CombatZone
    /// </summary>
    private void ValidateEnemyPositions()
    {
        if (CombatArea == null || zoneCollider == null) return;

        Bounds combatAreaBounds = zoneCollider.bounds;

        foreach (var enemyData in EnemiesInZone)
        {
            if (enemyData.enemyObject == null) continue;

            // Store the enemy's offset relative to the CombatZone
            Vector3 localOffset = enemyData.enemyObject.transform.localPosition;

            // Ensure enemies stay inside CombatZone bounds
            Vector3 worldPosition = transform.TransformPoint(localOffset);
            if (!combatAreaBounds.Contains(worldPosition))
            {
                enemyData.enemyObject.transform.localPosition = Vector3.zero; // Reset to center
                Debug.LogWarning($"{enemyData.enemyObject.name} was outside the Combat Zone and has been repositioned.");
            }
        }
    }



    private void OnDrawGizmos()
    {
        if (zoneCollider != null)
        {
            // Draw the CombatArea bounds
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(zoneCollider.bounds.center, zoneCollider.bounds.size);
        }

        if (PlayerPosition != null)
        {
            // Draw the PlayerPosition
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(PlayerPosition.transform.position, 0.5f);
        }

        if (CombatCameraPosition != null)
        {
            // Draw the CameraPosition
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(CombatCameraPosition.transform.position, 0.5f);
        }
    }
}
