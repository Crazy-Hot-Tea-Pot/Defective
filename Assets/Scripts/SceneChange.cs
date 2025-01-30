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

    private Levels nextLevel;
    private Quest quest;

    void Start()
    {
        NextLevel = GameManager.Instance.CurrentLevel;
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
            //load next scene
            GameManager.Instance.RequestScene(NextLevel);
        }
    }
}
