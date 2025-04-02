using UnityEngine;

public class PuzzleRange : MonoBehaviour
{
    public float currentHealth;
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
        //If the triggered object is the player
        if (other.tag == "Player")
        {
            //Then set the correct materal to the render component of this object expected to be the puzzle object
            this.gameObject.GetComponent<Renderer>().material = mat;
            //Open the puzzle UI
            PuzzleManager.Instance.OpenPuzzle(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //If the triggered object is the player
        if (other.tag == "Player")
        {
            //TThen set the correct materal to the render component this time adding back it's original materal
            this.gameObject.GetComponent<Renderer>().material = save;
            //Close the puzzle UI
            PuzzleManager.Instance.ClosePuzzle();
        }
    }

    public void TakeDamage(int number)
    {
        //Reduce the puzzles health by it's damage value
        currentHealth = currentHealth - number;
        //If that health is less then or 0
        if (currentHealth <= 0)
        {
            //Add to current quests a newQuest
            QuestManager.Instance.AddCurrentQuest(newQuest);
            //If the quest marker is not null
            if(QuestMarker != null)
            {
                //Set that active
                QuestMarker.SetActive(true);
            }
            //Close the puzzle UI
            PuzzleManager.Instance.ClosePuzzle();
            //Destroy this game object expected to be the puzzle object
            Destroy(this.gameObject);
        }
    }
}
