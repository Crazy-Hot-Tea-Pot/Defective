using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest : ScriptableObject
{
    public string questName;
    public string ccQuestName;
    public string modifiedQuestName;
    public string questDesc;
    public bool complete = false;
    public bool isTutorial;
    public bool mainQuest;
    public int scrap;

    private void Awake()
    {
        ccQuestName = questName;
    }
    /// <summary>
    /// A method intended to be overwritten to run quests and meet their requirments
    /// </summary>
    public virtual void RunQuest()
    {
        
    }

    ///<summary>
    ///This method is always over written but allows for mono objects using collision detection to pass useful data script by script
    ///</summary>
    public virtual void TouchPassThrough(string tag)
    {

    }

    /// <summary>
    /// A method intended to complete quests in some cases it can be overwritten
    /// </summary>
    public virtual void CompleteQuest()
    {
        complete = true;
        questName = "\n" + ccQuestName + " (Complete)";
        GameObject.Find("UiManager/Roaming And Combat UI/MiniBarSettingAndUi").GetComponent<QuestUIController>().AnimateLog();
        Debug.Log("Quest Complete: " + questName);
        GameObject.Find("Player").GetComponent<PlayerController>().GainScrap(scrap);
        //QuestManager.Instance.UpdateQuestHud(this);
    }

    /// <summary>
    /// A class meant to override for the count enemies quest
    /// </summary>
    /// <param name="enemyTypeName"></param>
    public virtual void EnemyQuestCounterUpdate(string enemyTypeName)
    {
        Debug.Log("EnemyCounterBase");
    }

    public virtual void TriggerPopup()
    {

    }

    public virtual void TriggerMovement()
    {

    }
}
