using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance
    {
        get
        {
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    private static QuestManager instance;

    [Tooltip("A list of all quests availiable in order")]
    public List<Quest> futureQuestList;
    public List<Quest> completeList;

    public List<Quest> CurrentQuest;

    private bool automatic = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //Load all the quest scriptables and clone them
        PopulateQuestSciptables();

        //If there are no current quests 
        if(CurrentQuest == null || CurrentQuest.Count == 0)
        {
            automatic = true;
        }

        //If no order is given for quests we will make one up
        if(automatic)
        {
            foreach (Quest quest in futureQuestList)
            {
                CurrentQuest.Add(quest);
            }
            futureQuestList.Clear();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //This line tells the manager to stop if we run out of quests
        if(CurrentQuest != null)
        {
            for (int i = 0; i < CurrentQuest.Count; i++)
            {
                    //If the current quest is complete
                    if (CurrentQuest[i].complete == true)
                    {
                        //Add to the complete list
                        completeList.Add(CurrentQuest[i]);
                        //remove it from current
                        CurrentQuest.RemoveAt(i);

                        //Change the current quest if it's not null
                        if (futureQuestList.Count != 0)
                        {
                            //Make the next quest current quest
                            CurrentQuest[i] = futureQuestList[0];
                            futureQuestList.RemoveAt(0);
                        }
                    }
                    else
                    {
                        CurrentQuest[i].RunQuest();
                    }
            }
        }
    }

    /// <summary>
    ///Retrieve quest information
    /// </summary>
    /// <param name="index"></param>
    /// <param name="quest"></param>
    /// <param name="description"></param>
    public void RetrieveQuestInfo(int index, TMP_Text quest, TMP_Text description)
    {
        //Try to collect a quest from the currentquest list
        try
        {
                quest.text = CurrentQuest[index].questName;
                description.text = CurrentQuest[index].questDesc;
        }
        //But if we can't that's ok just go to the complete list
        catch
        {
            //For some reason not having two try catches just will return null even with a != null check
            try
            {
                //If there is a value to show as a complete quest show it
                if (completeList[0] != null)
                {
                        quest.text = completeList[0].questName;
                        description.text = completeList[0].questDesc;
                }
            }
            //If there is only one quest then just hide the other text and make it nothing
            catch
            {
                quest.text = " ";
                description.text = " ";
                Debug.Log("That quest doesn't exist at index: " + index);
            }
        }
    }

    /// <summary>
    /// Retrieve the name and description of a quest based on a given index. Use -1 for current quest
    /// </summary>
    /// <param name="index"></param>
    /// <param name="quest"></param>
    public void RetrieveQuestInfo(int index, TMP_Text quest)
    {
        //Try to collect a quest from the currentquest list
        try
        {
            quest.text = CurrentQuest[index].questName;
        }
        //But if we can't that's ok just go to the complete list
        catch
        {
            //For some reason not having two try catches just will return null even with a != null check
            try
            {
                //If there is a value to show as a complete quest show it
                if (completeList[0] != null)
                {
                    quest.text = completeList[0].questName;
                }
            }
            //If there is only one quest then just hide the other text and make it nothing
            catch
            {
                quest.text = " ";
                Debug.Log("That quest doesn't exist at index: " + index);
            }
        }
    }

    #region Quest Information
    /// <summary>
    /// If the quest state is false that means it is not a completed quest
    /// </summary>
    /// <param name="questName"></param>
    /// <returns></returns>
    public bool CheckQuestState(Quest QuestToCheck)
    {
        string questName = QuestToCheck.questName;
        bool questState = false;
        foreach(Quest quest in completeList)
        {
            if(quest.questName == questName)
            {
                questState = true;
               
            }
            else
            {
                questState = false;
            }
        }

        return questState;
    }

    /// <summary>
    /// Trigger quest events
    /// </summary>
    /// <param name="quest"></param>
    /// <param name="eventName"></param>
    public void TriggerQuestEvent(Quest quest, string eventName)
    {
        if(eventName == "Complete" || eventName == "complete")
        {
            quest.CompleteQuest();
        }
    }

    /// <summary>
    /// Returns all quests
    /// </summary>
    /// <returns></returns>
    public List<Quest> GetAllQuests()
    {
        List<Quest> tempList = null;
        foreach (Quest quest in CurrentQuest)
        {
            tempList.Add(quest);
        }
        foreach (Quest quest in futureQuestList)
        {
            tempList.Add(quest);
        }
        foreach(Quest quest in completeList)
        {
            tempList.Add(quest);
        }
        return tempList;
    }

    /// <summary>
    /// Return all future quests but exclude quests that are complete or not yet active
    /// </summary>
    /// <returns></returns>
    public List<Quest> GetFutureQuests()
    {
        List<Quest> tempList;
        tempList = futureQuestList;
        return tempList;
    }

    /// <summary>
    /// Populates the quest scriptables
    /// </summary>
    public void PopulateQuestSciptables()
    {
        //Load all quests in the quest folder
        futureQuestList = new List<Quest>(Resources.LoadAll<Quest>("Scriptables/Quest"));

        //We don't want to effect the originals so we are instantly copying the quest into its self so it's a list of copiess
        for (int i = 0; i < futureQuestList.Count; i++)
        {

            //Clone the value
            Quest Temp = futureQuestList[i];
            futureQuestList[i] = Instantiate(Temp);
        }
    }



    #endregion
}
