using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestGoHere", menuName = "Quest/GoHere")]
public class QuestGoHere : Quest
{
    [Tooltip("The position of the player when this quest object is complete")]
    public string questEndNameForPositionCalc;

    private bool canEnd = false;

    private void Awake()
    {
        modifiedQuestName = questName;
        ccQuestName = modifiedQuestName;
    }
    //Override the Run quest method
    public override void RunQuest()
    {
        if (!complete)
        {

            if (questEndNameForPositionCalc == null)
            {
                Debug.Log("Quest has no end position and can not continue. I have completed the quest");
                CompleteQuest();
            }
            else
            {
                //if collided with player
                if(canEnd)
                {
                    CompleteQuest();
                }
            }
        }
    }

    //Overide complete quest to delete the queset object
    public override void CompleteQuest()
    {
        try
        {
            GameObject.Find(questEndNameForPositionCalc);
            //QuestManager.Instance.UpdateQuestHud(this);
            //Use the base of the method
            base.CompleteQuest();
        }
        catch
        {
            Debug.Log("Could not find " + questEndNameForPositionCalc);
        }
    }

    //Pass through can end to show collision has been made in QuestCollison
    public override void TouchPassThrough(string tag)
    {
        canEnd = true;
    }
}

