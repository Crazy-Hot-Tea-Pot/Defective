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

    public GameObject ConfirmationWindow;
    private Quest nextSpawnQuest;

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
        //If one is delete doubles in future quests
        else
        {
            //We don't want to effect the originals so we are instantly copying the quest into its self so it's a list of copiess
            for (int i = 0; i < CurrentQuest.Count; i++)
            {

                //Clone the value
                Quest Temp = CurrentQuest[i];
                CurrentQuest[i] = Instantiate(Temp);
            }

            //Remove doubles
            for (int i = 0; i < futureQuestList.Count; i++)
            {
                for(int y = 0; y < CurrentQuest.Count; y++)
                {
                    if (futureQuestList[i].name == CurrentQuest[y].name)
                    {
                        futureQuestList.RemoveAt(i);
                    }
                }
            }
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

                        //Change the current quest if it's not null and it's automatic
                        if (futureQuestList.Count != 0 && automatic == true)
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

    public void CreateConfirmationWindow(string text, Quest nextQuest, Quest questToComplete)
    {
        if(nextQuest != null)
        {
            nextSpawnQuest = nextQuest;
            GameObject window = Instantiate(ConfirmationWindow, UiManager.Instance.transform);
            window.GetComponent<ConfirmationWindow>().SetUpComfirmationWindow(text, SummonQuest);
            questToComplete.CompleteQuest();
        }
        else
        {
            GameObject window = Instantiate(ConfirmationWindow, UiManager.Instance.transform);
            window.GetComponent<ConfirmationWindow>().SetUpComfirmationWindow(text, null);
        }
    }

    public void CreateNullConfirmationWindow(string text, Quest completedQuest)
    {
        nextSpawnQuest = completedQuest;
        GameObject window = Instantiate(ConfirmationWindow, UiManager.Instance.transform);
        window.GetComponent<ConfirmationWindow>().SetUpComfirmationWindow(text, ForceComplete);
    }

    public void ForceComplete()
    {
        nextSpawnQuest.CompleteQuest();
    }

    public void SummonQuest()
    {
        AddCurrentQuest(nextSpawnQuest);
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

    #region Quest Information
    private string lastQuest;
    /// <summary>
    /// Retrieve the name and description of a quest based on a given index. Use -1 for current quest
    /// </summary>
    /// <param name="index"></param>
    /// <param name="quest"></param>
    public void RetrieveQuestInfo(int index, TMP_Text quest)
    {
        while (CurrentQuest[index].isTutorial || CurrentQuest[index].questName == lastQuest)
        {
            if(index + 1 <= CurrentQuest.Count)
            {
                index += 1;
            }
            else
            {
                break;
            }
            if (CurrentQuest.Count == index)
            {
                index -= 1;
                break;
            }
        }

        if (!CurrentQuest[index].isTutorial)
        {
            //Try to collect a quest from the currentquest list
            try
            {
                quest.text = CurrentQuest[index].questName;
                lastQuest = quest.text;
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
        //Else if there is a tutorial and it's the last thing left show it.
        else
        {
            //If there is a value to show as a complete quest show it
            if (completeList[index] != null)
            {
                quest.text = "No Quest Assigned";
            }
        }
       
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

    public void AddCurrentQuest(Quest quest)
    {
        if(quest != null)
        {
            Quest Temp = Instantiate(quest);
            quest = Temp;
            CurrentQuest.Add(Temp);
            if (futureQuestList.Contains(Temp))
            {
                futureQuestList.Remove(Temp);
            }
        }
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

    public void tutorialTrigger(string questName)
    {
        for (int i = 0; i < CurrentQuest.Count; i++)
        {
            if(CurrentQuest[i].questName == questName)
            {
                CurrentQuest[i].TriggerPopup();
            }
        }
    }

    public void tutorialMoveTrigger()
    {
        for (int i = 0; i < CurrentQuest.Count; i++)
        {
            CurrentQuest[i].TriggerMovement();
        }
    }


    #endregion
}
