using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetQuests : MonoBehaviour
{
    public List<Quest> StartingQuests;
    public bool canBeNull;
    // Start is called before the first frame update
    void Start()
    {
        QuestManager.Instance.CurrentQuest.Clear();
        QuestManager.Instance.futureQuestList.Clear();
        //Load all the quest scriptables and clone them
        PopulateQuestScriptables();

        //If there are no current quests 
        if (StartingQuests == null || StartingQuests.Count == 0)
        {
            if (canBeNull == false)
            {
                QuestManager.Instance.automatic = true;
            }
        }

        //If no order is given for quests we will make one up
        if (QuestManager.Instance.automatic)
        {
            foreach (Quest quest in QuestManager.Instance.futureQuestList)
            {
                QuestManager.Instance.CurrentQuest.Add(quest);
            }
            QuestManager.Instance.futureQuestList.Clear();
        }
        //If one is delete doubles in future quests
        else
        {
            //We don't want to effect the originals so we are instantly copying the quest into its self so it's a list of copiess
            for (int i = 0; i < StartingQuests.Count; i++)
            {

                //Clone the value
                Quest Temp = StartingQuests[i];
                QuestManager.Instance.CurrentQuest.Add(Instantiate(Temp));
            }

            //Remove doubles
            for (int i = 0; i < QuestManager.Instance.futureQuestList.Count; i++)
            {
                for (int y = 0; y < QuestManager.Instance.CurrentQuest.Count; y++)
                {
                    if (QuestManager.Instance.futureQuestList[i].name == QuestManager.Instance.CurrentQuest[y].name)
                    {
                        QuestManager.Instance.futureQuestList.RemoveAt(i);
                    }
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Populates the quest scriptables
    /// </summary>
    public void PopulateQuestScriptables()
    {
        //Load all quests in the quest folder
        QuestManager.Instance.futureQuestList = new List<Quest>(Resources.LoadAll<Quest>("Scriptables/Quest"));

        //We don't want to effect the originals so we are instantly copying the quest into its self so it's a list of copiess
        for (int i = 0; i < QuestManager.Instance.futureQuestList.Count; i++)
        {

            //Clone the value
            Quest Temp = QuestManager.Instance.futureQuestList[i];
            QuestManager.Instance.futureQuestList[i] = Instantiate(Temp);
        }
    }
}
