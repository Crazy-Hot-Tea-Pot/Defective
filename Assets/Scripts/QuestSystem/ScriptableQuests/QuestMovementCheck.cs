using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestMovementCheck", menuName = "Quest/MovementCheck")]
public class QuestMovementCheck : Quest
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void TriggerMovement()
    {
        Destroy(GameObject.Find("MouseClick"));
        CompleteQuest();
    }
}
