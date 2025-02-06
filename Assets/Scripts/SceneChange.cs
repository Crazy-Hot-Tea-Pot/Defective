using UnityEngine;

public class SceneChange : MonoBehaviour
{
    /// <summary>
    /// Stores the level name as a string
    /// </summary>
    public Levels NextLevel
    {
        get
        {
            return nextLevel;
        }
        private set
        {
            nextLevel = value;
        }
    }
    [SerializeField]
    private Levels nextLevel;
    private Quest quest;

    void Start()
    {
        
    }

    public void SetNextLevel(Levels level,Quest OptionalQuest=null )
    {
        NextLevel = level;
        quest = OptionalQuest;
    }

    private void OnTriggerEnter(Collider other)
    {        
        //TODO add quest condition
        if (other.transform.tag == "Player") //&& QuestManager.Instance)
        {
            // Update StoryManager with the new level BEFORE switching scenes
            StoryManager.Instance.SetNextLevel(nextLevel);            

            //load next scene
            GameManager.Instance.RequestScene(NextLevel);
        }
    }
}
