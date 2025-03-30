using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "QuestUIHighlight", menuName = "Quest/QuestUIHighlight")]
public class QuestUIHighlight : Quest
{
    public string UIElementPath;
    private GameObject UIElement;
    public GameObject curserArrow;
    public int xOffset;
    public int yOffset;
    [Tooltip("Option quests if you put nothing here or don't fill both variables it will not cause a problem")]
    public Quest nextQuest = null;
    public Quest secondQuest = null;
    public bool InCombat;
    private bool IExist = false;

    private void Awake()
    {
        modifiedQuestName = questName;
        ccQuestName = modifiedQuestName;
    }
    public override void RunQuest()
    {
        try
        {
            //Find the UIElementPath
            UIElement = GameObject.Find(UIElementPath).gameObject;

            //If we have a button to change
            if (UIElement.GetComponent<Button>() == true)
            {

                //Add a button component
                UIElement.GetComponent<Button>().onClick.RemoveListener(ButtonCheck);
                UIElement.GetComponent<Button>().onClick.AddListener(ButtonCheck);
            }

            if (IExist == false && UIElement.activeSelf == true && InCombat == false)
            {
                IExist = true;
                curserArrow = Instantiate(curserArrow, UIElement.transform.parent.Find(UIElement.name));
                curserArrow.transform.position = new Vector3(curserArrow.transform.position.x + xOffset, curserArrow.transform.position.y + yOffset, curserArrow.transform.position.z);

            }
            else if (IExist == false && GameManager.Instance.CurrentGameMode == GameManager.GameMode.Combat)
            {
                IExist = true;
                curserArrow = Instantiate(curserArrow, UIElement.transform.parent.Find(UIElement.name));
                curserArrow.transform.position = new Vector3(curserArrow.transform.position.x + xOffset, curserArrow.transform.position.y + yOffset, curserArrow.transform.position.z);
            }

        }
        catch
        {
            Debug.LogError("SABASTIAN ERROR FROM CALLING IN UPDATE EVERYFRAME");
        }

    }

    public void ButtonCheck()
    {
        UIElement.GetComponent<Button>().onClick.RemoveListener(ButtonCheck);
        Destroy(curserArrow);
        QuestManager.Instance.AddCurrentQuest(nextQuest);
        QuestManager.Instance.AddCurrentQuest(secondQuest);
        CompleteQuest();
    }
}
