using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public Dialogue dialogueData;

    public GameObject PlayerPosition;

    [Header("Player Position (XZ editable, Y auto-adjusts to bottom of CombatArea)")]
    public Vector2 playerPositionXZOffset = Vector2.zero;

    private PlayerController playerController;

    void OnValidate()
    {
        if (PlayerPosition != null)
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                Vector3 triggerBottom = collider.bounds.min; // Get the lowest Y point of the collider

                PlayerPosition.transform.position = new Vector3(
                    triggerBottom.x + playerPositionXZOffset.x,
                    triggerBottom.y + 1f,
                    triggerBottom.z + playerPositionXZOffset.y
                );

                Debug.Log($"[DialogueTrigger] Updated PlayerPosition to {PlayerPosition.transform.position}");
            }
            else
            {
                Debug.LogWarning("[DialogueTrigger] No Collider found! Cannot position PlayerPosition correctly.");
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        PlayerPosition.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && dialogueData != null)
        {
            GameManager.Instance.UpdateGameMode(GameManager.GameMode.Dialogue);
            DialogueManager.Instance.StartDialogue(dialogueData);
            playerController.MovePlayerToPosition(PlayerPosition.transform.position);
            Destroy(this.gameObject, 1f);
        }
    }
}
