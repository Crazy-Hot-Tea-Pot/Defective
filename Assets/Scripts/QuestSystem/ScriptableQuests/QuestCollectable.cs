using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestGoHere", menuName = "Quest/QuestCollectable")]
public class QuestCollectable : Quest
{
    public string tagName;
    public int neededCollections;
    private int currentCollectoin = 0;
    private string QuestNameHold;
    private string QuestDescHold;

    private void Awake()
    {
        QuestDescHold = questDesc;
        QuestNameHold = questName;
        ccQuestName = QuestNameHold;
    }
    public override void RunQuest()
    {
        base.RunQuest();

        //Update the description
        questDesc = QuestDescHold +"\n" + "(" + currentCollectoin + "/" + neededCollections + ")";
        //Update the name
        questName = QuestNameHold + "(" + currentCollectoin + "/" + neededCollections + ")";
        //Update alt name
        modifiedQuestName = QuestNameHold;

        //If remaining enemies is equal to 0 complete the quest
        if (currentCollectoin == neededCollections)
        {
            questDesc = QuestDescHold;
            questName = QuestNameHold;
            //QuestManager.Instance.UpdateQuestHud(this);
            CompleteQuest();
        }
    }

    public override void TouchPassThrough(string tag)
    {
        base.TouchPassThrough(tag);
        if(tag == tagName)
        {
            currentCollectoin = currentCollectoin + 1;
            GameObject.Find("UiManager/Roaming And Combat UI/MiniBarSettingAndUi").GetComponent<QuestUIController>().AnimateLog();
            //QuestManager.Instance.UpdateQuestHud(this);
        }
    }
}
