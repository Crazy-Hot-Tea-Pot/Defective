using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest : ScriptableObject
{
    public string questName;
    private string ccQuestName;
    public string questDesc;
    public bool complete = false;

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
        questName = ccQuestName + " (Complete)";
        Debug.Log("Quest Complete: " + questName);
    }

    /// <summary>
    /// A class meant to override for the count enemies quest
    /// </summary>
    /// <param name="enemyTypeName"></param>
    public virtual void EnemyQuestCounterUpdate(string enemyTypeName)
    {

    }

}

