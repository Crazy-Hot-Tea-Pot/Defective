using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            //Give enemy counter update the enemy name so they can verify it
            foreach (Quest quest in QuestManager.Instance.CurrentQuest)
            {
                quest.TouchPassThrough(this.gameObject.transform.tag);
            }
            Destroy(this.gameObject);
        }
    }
}
