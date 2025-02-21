using UnityEngine;

public class PuzzleRange : MonoBehaviour
{
    private float currentHealth;
    public float maxHealth;
    public Quest newQuest;
    public Material mat;
    public string QuestMarkerName;
    private GameObject QuestMarker;
    private Material save;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        save = this.gameObject.GetComponent<Renderer>().material;
        try
        {
            QuestMarker = GameObject.Find(QuestMarkerName);
            QuestMarker.SetActive(false);
        }
        catch
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            this.gameObject.GetComponent<Renderer>().material = mat;
            PuzzleManager.Instance.OpenPuzzle(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            this.gameObject.GetComponent<Renderer>().material = save;
            PuzzleManager.Instance.ClosePuzzle(this.gameObject);
        }
    }

    public void TakeDamage(int number)
    {
        currentHealth = currentHealth - number;
        if (currentHealth <= 0)
        {
            QuestManager.Instance.AddCurrentQuest(newQuest);
            if(QuestMarker != null)
            {
                QuestMarker.SetActive(true);
            }
            DestroyMe();
        }
    }

    public void DestroyMe()
    {
        Destroy(this.gameObject);
    }
}
