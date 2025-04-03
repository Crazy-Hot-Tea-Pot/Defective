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
    private Quest secondQuestVar;

    public bool automatic = false;

    public int questIndex;

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
       
    }

    // Update is called once per frame
    void Update()
    {
        //This line tells the manager to stop if we run out of quests
        if(CurrentQuest != null)
        {
            for (int i = 0; i < CurrentQuest.Count; i++)
            {
                //This try stops a really annoying bug that appears inconsistently
                try
                {
                    //If the current quest is complete
                    if (CurrentQuest[i].complete == true)
                    {
                        //Add to the complete list
                        completeList.Add(CurrentQuest[i]);
                        //remove it from current
                        CurrentQuest.RemoveAt(i);
                        //Run the next quest on ui (NEVER DELETE THIS IT'S WAY MORE ESSENTIAL THEN IT LOOKS)
                        UpdateQuestHud(CurrentQuest[i]);

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
                        UpdateQuestHud(CurrentQuest[i]);
                    }
                }
                //If we index out of range randomly this only happens when we are done so just break out
                catch
                {
                    break;
                }
            }
        }
    }

    public void CreateConfirmationWindow(string text, Quest nextQuest, Quest questToComplete)
    {
        if(nextQuest != null)
        {
            nextSpawnQuest = nextQuest;
            // sure i told you to use the UIManager
            //GameObject window = Instantiate(ConfirmationWindow, UiManager.Instance.transform);
            //window.GetComponent<ConfirmationWindow>().SetUpComfirmationWindow(text, SummonQuest);

            UiManager.Instance.PopUpMessage(text, SummonQuest, false);

            //So much easier

            questToComplete.CompleteQuest();
        }
        else
        {
            //sigh why the f would you pass a null?
            //GameObject window = Instantiate(ConfirmationWindow, UiManager.Instance.transform);
            //window.GetComponent<ConfirmationWindow>().SetUpComfirmationWindow(text, null);

            UiManager.Instance.PopUpMessage(text);
        }
    }

    public void CreateConfirmationWindow(string text, Quest nextQuest, Quest SecondaryQuest, Quest questToComplete)
    {
        if (nextQuest != null && SecondaryQuest != null)
        {
            nextSpawnQuest = nextQuest;
            Debug.Log(nextQuest);
            secondQuestVar = SecondaryQuest;
            Debug.Log(SecondaryQuest);
            //GameObject window = Instantiate(ConfirmationWindow, UiManager.Instance.transform);
            //window.GetComponent<ConfirmationWindow>().SetUpComfirmationWindow(text, SummonQuest);
            UiManager.Instance.PopUpMessage(text, SummonQuest,false);
            questToComplete.CompleteQuest();
        }
        else
        {
            //GameObject window = Instantiate(ConfirmationWindow, UiManager.Instance.transform);
            //window.GetComponent<ConfirmationWindow>().SetUpComfirmationWindow(text, null);

            UiManager.Instance.PopUpMessage(text, null, false);
        }
    }

    public void CreateNullConfirmationWindow(string text, Quest completedQuest)
    {
        nextSpawnQuest = completedQuest;
        //GameObject window = Instantiate(ConfirmationWindow, UiManager.Instance.transform);
        //window.GetComponent<ConfirmationWindow>().SetUpComfirmationWindow(text, ForceComplete);

        UiManager.Instance.PopUpMessage(text, ForceComplete,false);
    }

    public void ForceComplete()
    {
        nextSpawnQuest.CompleteQuest();
    }

    public void SummonQuest()
    {
        if(nextSpawnQuest != null)
        {
            AddCurrentQuest(nextSpawnQuest);
            nextSpawnQuest = null;
        }
        if (secondQuestVar != null)
        {
            AddCurrentQuest(secondQuestVar);
            secondQuestVar = null;
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

    #region Quest Information
    private string lastQuest;
    /// <summary>
    /// Retrieve the name and description of a quest based on a given index. Use -1 for current quest
    /// </summary>
    /// <param name="index"></param>
    /// <param name="quest"></param>
    public void RetrieveQuestInfo(int index, TMP_Text quest)
    {

        index = IndexEditor(index);
        try
        {
            if (!CurrentQuest[index].isTutorial && CurrentQuest[index].mainQuest)
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

                    quest.text = " ";
                    Debug.Log("That quest doesn't exist at index: " + index);
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
        catch
        {
            quest.text = "No Quest Assigned";
        }
    }

    public void UpdateQuestHud(Quest quest)
    {
            if (quest.mainQuest && quest.name == CurrentQuest[questIndex].name)
            {
                if (quest.complete)
                {
                    GameObject.Find("UiManager/Roaming And Combat UI/MiniBarSettingAndUi").GetComponent<QuestUIController>().GenerateQuestLog(quest);
                }
                else
                {
                    GameObject.Find("UiManager/Roaming And Combat UI/MiniBarSettingAndUi").GetComponent<QuestUIController>().GenerateQuestLog();
                }
            }
    }

    public int IndexEditor(int index)
    {
        try
        {
            while (CurrentQuest[index].mainQuest == false || CurrentQuest[index].complete == true)
            {
                if (index + 1 < CurrentQuest.Count)
                {
                    index += 1;
                    Debug.Log(index + "Break");
                }
                else
                {
                    break;
                }
            }
            return index;
        }
        catch
        {
            index = 0;
            return index;
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
