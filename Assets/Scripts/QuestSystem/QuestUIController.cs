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
    private Button OpenSettingsbtn;
    private Button Morebtn;
    private GameObject MiniLogContainer;

    //Layer two
    private GameObject FullLogContainer;
    private Button ShowLessbtn;
    private Button ShowCompletebtn;

    //Layer three
    private GameObject ShowCompleteContainer;
    private Button Activebtn;

    //Animation Speed
    public float AnimationSpeed;

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
        //Options menu button
        OpenSettingsbtn = QuestUIContanier.transform.Find("OpenSettingsbtn").GetComponent<Button>();
        //Remove any listeners
        OpenSettingsbtn.onClick.RemoveAllListeners();
        //Find the method we need in settingsuicontroller and call it
        OpenSettingsbtn.onClick.AddListener(UiManager.Instance.ToggleSettings);
        MiniLogContainer = QuestUIContanier.transform.Find("MiniLog").gameObject;
        //Disable MiniLogContainer
        MiniLogContainer.SetActive(false);
        FullLogContainer = QuestUIContanier.transform.Find("FullLog").gameObject;
        //Disable full log
        FullLogContainer.SetActive(false);
        ShowCompleteContainer = QuestUIContanier.transform.Find("CompleteList").gameObject;
        //Disable complete list
        ShowCompleteContainer.SetActive(false);

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
        //Close the mini log container
        MiniLogContainer.SetActive(false);

        //Open full log
        FullLogContainer.SetActive(true);
        GenerateQuestLog(FullLogContainer);

        //Show less button found
        ShowLessbtn = FullLogContainer.transform.Find("Lessbtn").GetComponent<Button>();
        //Remove listeners for doubles
        ShowLessbtn.onClick.RemoveAllListeners();
        //Add a listener for show less
        ShowLessbtn.onClick.AddListener(OpenMiniLog);

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
                    //Add one by one every item in the complete list
                    textBox.text = textBox.text + "\n " + quest.questName + "\n " + quest.questDesc;
                }
            }
        }
        //If the container is open
        else if (ShowCompleteContainer.activeSelf == true)
        {
            //Close it
            ShowCompleteContainer.SetActive(false);
        }
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
            //Create button for more
            Morebtn = MiniLogContainer.transform.Find("Morebtn").GetComponent<Button>();
            //Remove doubled listeners
            Morebtn.onClick.RemoveAllListeners();
            //Add a listener to open the full log
            Morebtn.onClick.AddListener(OpenFullLog);
            //Close large log if open. We use a null check because it can be
            if (FullLogContainer != null)
            {
                FullLogContainer.SetActive(false);
            }
            //Animate it to pull out
            //while (true)
            //{
            //    //If minilogcontainer is not less then or equal to 195
            //    if (MiniLogContainer.transform.position.x != 231)
            //    {
            //        Debug.Log(AnimationSpeed);
            //         Debug.Log(MiniLogContainer.transform.position);
            //        //Move the container left by animation speed
            //        MiniLogContainer.transform.position = new Vector3(MiniLogContainer.transform.position.x - AnimationSpeed, MiniLogContainer.transform.position.y, MiniLogContainer.transform.position.z);
            //        Debug.Log(MiniLogContainer.transform.position);
            //    }
            //    //Reset to correct position if we go to far
            //    if (MiniLogContainer.transform.position.x < 231)
            //    {
            //        //Move it to 195
            //        MiniLogContainer.transform.position = new Vector3(231, MiniLogContainer.transform.position.y, MiniLogContainer.transform.position.z);
            //    }
            //    if (MiniLogContainer.transform.position.x == 231)
            //    {
            //        break;
            //    }

            //    //Animate the button
            //    if (OpenLogbtn.gameObject.transform.position.x != -416)
            //    {
            //        //Move left by animation speed
            //        OpenLogbtn.gameObject.transform.position = new Vector3(OpenLogbtn.gameObject.transform.position.x - AnimationSpeed, OpenLogbtn.gameObject.transform.position.y, OpenLogbtn.gameObject.transform.position.z);
            //    }
            //    //If the button is moved to far reset it
            //    if (OpenLogbtn.gameObject.transform.position.x < -416)
            //    {
            //        OpenLogbtn.gameObject.transform.position = new Vector3(-416, OpenLogbtn.gameObject.transform.position.y, OpenLogbtn.transform.position.z);
            //    }
            //    if (OpenLogbtn.gameObject.transform.position.x == -416)
            //    {
            //        break;
            //    }
            //}

            }
        //If open close it
        else if(MiniLogContainer.activeSelf == true)
        {
            ////Animate it to push in

            //    //Animate the button
            //    if (OpenLogbtn.gameObject.transform.position.x < 839.6284f)
            //    {
            //        //Move left by animation speed
            //        OpenLogbtn.gameObject.transform.position = new Vector3(OpenLogbtn.transform.position.x - AnimationSpeed * 3, OpenLogbtn.gameObject.transform.position.y, OpenLogbtn.gameObject.transform.position.z);
            //    }
            //    //If the button is moved to far reset it
            //    else if (OpenLogbtn.gameObject.transform.position.x == 839.6284f)
            //    {
            //        OpenLogbtn.gameObject.transform.position = new Vector3(839.6284f, OpenLogbtn.gameObject.transform.position.y, OpenLogbtn.transform.position.z);

            //    }

            //    //If minilogcontainer is not more then or equal to 1495
            //    if (MiniLogContainer.transform.position.x < 1495)
            //    {
            //        //Move the container right by animation speed * 3
            //        MiniLogContainer.transform.position = new Vector3(MiniLogContainer.transform.position.x - AnimationSpeed * 3, MiniLogContainer.transform.position.y, MiniLogContainer.transform.position.z);
            //    }
            //    //When in position  hide it
            //    else if (MiniLogContainer.transform.position.x == 1495)
            //    {
            MiniLogContainer.SetActive(false);
            //    }


        }
    }

    public void GenerateQuestLog(GameObject logContainer)
    {
        //Generate log for mini log
        if(logContainer == MiniLogContainer)
        {
            //A variable in quest manager to stop repeating quests it causes problems if we shift pannels between mini and full log so it must be cleared
            QuestManager.Instance.nameTemp = " ";
            //Add a container for quest one
            Quest1 = logContainer.transform.Find("Quest1").GetComponent<TMP_Text>();
            QuestManager.Instance.RetrieveQuestInfo(0, Quest1);

            //Add a container for quest two
            Quest2 = logContainer.transform.Find("Quest2").GetComponent<TMP_Text>();
            QuestManager.Instance.RetrieveQuestInfo(1, Quest2);
        }
        else if (logContainer == FullLogContainer)
        {
            //A variable in quest manager to stop repeating quests it causes problems if we shift pannels between mini and full log so it must be cleared
            QuestManager.Instance.nameTemp = " ";
            //Add a container for quest one
            Quest1 = logContainer.transform.Find("Quest1").GetComponent<TMP_Text>();
            Quest1Desc = logContainer.transform.Find("Quest1LongDescription").GetComponent<TMP_Text>();
            QuestManager.Instance.RetrieveQuestInfo(0, Quest1,Quest1Desc);

            //Add a container for quest two
            Quest2 = logContainer.transform.Find("Quest2").GetComponent<TMP_Text>();
            Quest2Desc = logContainer.transform.Find("Quest2LongDescription").GetComponent<TMP_Text>();
            QuestManager.Instance.RetrieveQuestInfo(1, Quest2, Quest2Desc);

            //Add a container for quest three
            Quest3 = logContainer.transform.Find("Quest3").GetComponent<TMP_Text>();
            Quest3Desc = logContainer.transform.Find("Quest3LongDescription").GetComponent<TMP_Text>();
            QuestManager.Instance.RetrieveQuestInfo(3, Quest3, Quest3Desc);

            //Add a container for quest four
            Quest4 = logContainer.transform.Find("Quest4").GetComponent<TMP_Text>();
            Quest4Desc = logContainer.transform.Find("Quest4LongDesciption").GetComponent<TMP_Text>();
            QuestManager.Instance.RetrieveQuestInfo(4, Quest4, Quest4Desc);
        }
    }
}
