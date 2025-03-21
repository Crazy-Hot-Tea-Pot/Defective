using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUIController : MonoBehaviour
{
    //Layer One UI
    private GameObject QuestUIContanier;
    private Button OpenLogbtn;
    private GameObject MiniLogContainer;

    //Animation stuff
    public Animator animationController;

    //Text Variables
    private TMP_Text Quest1;
    private TMP_Text Quest1Desc;
    private TMP_Text Quest2;
    private TMP_Text Quest2Desc;
    private TMP_Text Quest3;
    private TMP_Text Quest3Desc;
    private TMP_Text Quest4;
    private TMP_Text Quest4Desc;

    // Start is called before the first frame update
    void Start()
    {
        //Locate containers
        QuestUIContanier = this.gameObject;
        OpenLogbtn = QuestUIContanier.transform.Find("OpenLogbtn").GetComponent<Button>();
        //Remove all listeners incase of doubles
        OpenLogbtn.onClick.RemoveAllListeners();
        //Add a listener for openMinILog
        OpenLogbtn.onClick.AddListener(OpenMiniLog);

        MiniLogContainer = QuestUIContanier.transform.Find("Mask").gameObject.transform.Find("MiniLog").gameObject;
        //Disable MiniLogContainer
        MiniLogContainer.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Open and animate the mini log
    /// </summary>
    public void OpenMiniLog()
    {
        //If the miniLog is closed open it
        if (MiniLogContainer.activeSelf == false)
        {
            //Set mini log visible
            MiniLogContainer.SetActive(true);

            GenerateQuestLog(MiniLogContainer);

            //Animate it to pull out
            animationController.SetTrigger("Push");

        }
        //If open close it
        else if(MiniLogContainer.activeSelf == true)
        {
            //Animate it to push in
            animationController.SetTrigger("Pull");

            StartCoroutine(WaitForMinilogContainerDIsable(1.5f));

        }
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

    public void GenerateQuestLog(GameObject logContainer)
    {
        //Generate log for mini log
        if (logContainer == MiniLogContainer)
        {
            //Add a container for quest one
            Quest1 = logContainer.transform.Find("Quest1").GetComponent<TMP_Text>();
            QuestManager.Instance.RetrieveQuestInfo(0, Quest1);

            //Add a container for quest two
            Quest2 = logContainer.transform.Find("Quest2").GetComponent<TMP_Text>();
            QuestManager.Instance.RetrieveQuestInfo(1, Quest2);
        }
    }
}
