using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestLogController : MonoBehaviour
{
    private GameObject QuestUIContanier;
    private Button BackButton;
    //Layer One
    private GameObject FullLogContainer;
    private Button ShowCompletebtn;

    //Layer Two
    private GameObject ShowCompleteContainer;
    private Button Activebtn;
    // Start is called before the first frame update
    void Start()
    {
        QuestUIContanier = this.gameObject;
        FullLogContainer = QuestUIContanier.transform.Find("FullLog").gameObject;
        //Disable full log
        FullLogContainer.SetActive(false);
        ShowCompleteContainer = QuestUIContanier.transform.Find("CompleteList").gameObject;
        //Disable complete list
        ShowCompleteContainer.SetActive(false);
        QuestUIContanier.SetActive(false);
        BackButton = QuestUIContanier.transform.Find("Backbtn").GetComponent<Button>();
        BackButton.onClick.RemoveAllListeners();
        BackButton.onClick.AddListener(CloseMenu);
    }

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Open full log
    /// </summary>
    public void OpenFullLog()
    {

        //Open full log
        FullLogContainer.SetActive(true);
        GenerateQuestLog(FullLogContainer);

        //Find show complete button
        ShowCompletebtn = FullLogContainer.transform.Find("Completebtn").GetComponent<Button>();
        //Remove listeners
        ShowCompletebtn.onClick.RemoveAllListeners();
        //Add a listener
        ShowCompletebtn.onClick.AddListener(ShowComplete);

    }


    /// <summary>
    /// Switch the quests to show complete
    /// </summary>
    public void ShowComplete()
    {
        //If the container is closed
        if (ShowCompleteContainer.activeSelf == false)
        {
            //Open it
            ShowCompleteContainer.SetActive(true);

            //Find the button to go back
            Activebtn = ShowCompleteContainer.transform.Find("Activebtn").GetComponent<Button>();
            Activebtn.onClick.RemoveAllListeners();
            Activebtn.onClick.AddListener(ShowComplete);

            //Find the text box for scroll content
            TMP_Text textBox;
            textBox = ShowCompleteContainer.transform.Find("Scroll View").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
            //Clear the text box in content
            textBox.text = " ";

            //If the complete list is empty tell the player
            if (QuestManager.Instance.completeList.Count == 0 || QuestManager.Instance.completeList == null)
            {
                textBox.text = "No Completed Quests";
            }
            //Otherwise continue
            else
            {
                //Go through our complete list
                foreach (Quest quest in QuestManager.Instance.completeList)
                {
                    if (quest.isTutorial != true)
                    {
                        //Add one by one every item in the complete list
                        textBox.text = textBox.text + "\n " + quest.questName + "\n " + quest.questDesc;
                    }
                }
            }

            //A needed fail safe when quests run out especially if there is a tutorial quest in que
            if (textBox.text == " ")
            {
                textBox.text = "No Completed Quests";
            }
        }
        //If the container is open
        else if (ShowCompleteContainer.activeSelf == true)
        {
            //Close it
            ShowCompleteContainer.SetActive(false);
        }
    }

    public void GenerateQuestLog(GameObject logContainer)
    {
        if (logContainer == FullLogContainer)
        {
            //Find the text box for scroll content
            TMP_Text textBox;
            textBox = logContainer.transform.Find("Scroll View").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
            //Clear the text box in content
            textBox.text = " ";

            //If the complete list is empty tell the player
            if (QuestManager.Instance.CurrentQuest.Count == 0 || QuestManager.Instance.CurrentQuest == null)
            {
                textBox.text = "No Active Quests";
            }
            //Otherwise continue
            else
            {
                //Go through our complete list
                foreach (Quest quest in QuestManager.Instance.CurrentQuest)
                {
                    if (quest.isTutorial != true)
                    {
                        //Add one by one every item in the complete list
                        textBox.text = textBox.text + "\n " + quest.modifiedQuestName + "\n " + quest.questDesc + "\n";
                    }
                }
            }

            //A needed fail safe when quests run out especially if there is a tutorial quest in que
            if (textBox.text == " ")
            {
                textBox.text = "No Active Quests";
            }
        }
    }

    public void CloseMenu()
    {
        FullLogContainer.SetActive(false);
        ShowCompleteContainer.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
