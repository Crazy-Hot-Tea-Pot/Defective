using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUIController : MonoBehaviour
{
    //Layer One UI
    private GameObject QuestUIContanier;
    private GameObject MiniLogContainer;

    //Animation stuff
    public Animator animationController;

    //Text Variables
    private TMP_Text Quest1;

    // Start is called before the first frame update
    void Start()
    {
        //Locate containers
        QuestUIContanier = this.gameObject;
        MiniLogContainer = QuestUIContanier.transform.Find("MiniLog").gameObject;
        GenerateQuestLog();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Meaninful for animation
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator WaitForMinilogContainerDIsable(float time)
    {
        yield return new WaitForSeconds(time);
        //Deactivate done in animator
        MiniLogContainer.SetActive(false);
    }

    public void GenerateQuestLog()
    {
            //Add a container for quest one
            Quest1 = MiniLogContainer.transform.Find("Quest1").GetComponent<TMP_Text>();
            QuestManager.Instance.RetrieveQuestInfo(0, Quest1);

            //Add a container for quest two
            //Quest2 = logContainer.transform.Find("Quest2").GetComponent<TMP_Text>();
            //QuestManager.Instance.RetrieveQuestInfo(1, Quest2);
        
    }
}
