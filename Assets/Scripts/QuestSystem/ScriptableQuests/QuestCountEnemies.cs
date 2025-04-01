using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestCountEnemies", menuName = "Quest/QuestCountEnemies")]
public class QuestCountEnemies : Quest
{
    public int totalEnemies;
    private int remainingEnemies = 0;
    public string enemyType;

    private string saveQuestName;
    private string saveQuestDesc;

    private void Awake()
    {
        saveQuestDesc = questDesc;
        saveQuestName = questName;
        ccQuestName = saveQuestName;
    }
    public override void RunQuest()
    {

        //Update the description
        questDesc = saveQuestDesc + "Kill " + enemyType + "\n(" + remainingEnemies + "/" + totalEnemies + ")";
        //Update the name
        questName = saveQuestName + "Kill " + enemyType + " (" + remainingEnemies + "/" + totalEnemies + ")";
        //Update alt name
        modifiedQuestName = saveQuestName + "Kill " + enemyType;

        //If remaining enemies is equal to 0 complete the quest
        if (remainingEnemies == totalEnemies)
        {
            questDesc = saveQuestDesc;
            questName = saveQuestName;
            CompleteQuest();
        }

    }

    /// <summary>
    /// When an enemy dies hand the counter quest it's name if it's the right enemy incriment the counter. IMPORTANT!! I highly recomend using this in a try catch.
    /// </summary>
    /// <param name="name">A varaible to compare against the saved name in the quest. This should be the exact name of the enemy type you are looking for example Looter not Looter(clonen)</param>
    public override void EnemyQuestCounterUpdate(string enemyTypeName)
    {
        //If the name matches
        if(enemyTypeName == enemyType)
        {
            //Take away from the counter
            remainingEnemies += 1;
            GameObject.Find("UiManager/Roaming And Combat UI/MiniBarSettingAndUi").GetComponent<QuestUIController>().AnimateLog();
            //QuestManager.Instance.UpdateQuestHud(this);
        }
    }
}
