using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestGoHere", menuName = "Quest/QuestCollectable")]
public class QuestCollectable : Quest
{
    public string tagName;
    public int neededCollections;
    private int currentCollectoin = 0;
    string ccQuestName;
    string ccQuestDesc;

    private void Awake()
    {
        ccQuestDesc = questDesc;
        ccQuestName = questName;
    }
    public override void RunQuest()
    {
        base.RunQuest();

        //Update the description
        questDesc = ccQuestDesc + "(" + ccQuestDesc + "/" + neededCollections + ")";
        //Update the name
        questName = ccQuestName + "(" + currentCollectoin + "/" + neededCollections + ")";

        //If remaining enemies is equal to 0 complete the quest
        if (currentCollectoin == neededCollections)
        {
            questDesc = ccQuestDesc;
            questName = ccQuestName;
            CompleteQuest();
        }
    }

    public override void TouchPassThrough(string tag)
    {
        base.TouchPassThrough(tag);
        if(tag == tagName)
        {
            currentCollectoin = currentCollectoin + 1;
        }
    }
}
