using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "QuestUIHighlight", menuName = "Quest/QuestUIHighlight")]
public class QuestUIHighlight : Quest
{
    public Material mat;
    [Header("This will find and utalize a button")]
    public string UIElementPath;
    private GameObject UIElement;
    public GameObject curserArrow;
    public int xOffset;
    public int yOffset;
    public string PopupText;
    public bool PopUp;
    public bool InCombat;
    public bool Index;
    public int TopIndex;
    public int CurrentIndex;

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
                ////Get the image component and apply our material
                //UIElement.GetComponent<Image>().material = mat;

                //Add a button component
                UIElement.GetComponent<Button>().onClick.RemoveListener(ButtonCheck);
                UIElement.GetComponent<Button>().onClick.AddListener(ButtonCheck);
            }

            if (GameObject.Find(UIElementPath).transform.Find(curserArrow.name) == null && UIElement.activeSelf == true && InCombat == false)
            {
                curserArrow = Instantiate(curserArrow, UIElement.transform.parent.Find(UIElement.name));
                curserArrow.transform.position = new Vector3(curserArrow.transform.position.x + xOffset, curserArrow.transform.position.y + yOffset, curserArrow.transform.position.z);

                if (PopUp)
                {
                    if (Index)
                    {
                        QuestManager.Instance.CreateConfirmationWindow(PopupText, QuestManager.Instance.IncreaseWindowIndex, TopIndex, CurrentIndex);
                    }
                    else
                    {
                        QuestManager.Instance.CreateConfirmationWindow(PopupText, null);
                    }
                }
            }
            else if (GameObject.Find(UIElementPath).transform.Find(curserArrow.name) == null && GameManager.Instance.CurrentGameMode == GameManager.GameMode.Combat)
            {
                curserArrow = Instantiate(curserArrow, UIElement.transform.parent.Find(UIElement.name));
                curserArrow.transform.position = new Vector3(curserArrow.transform.position.x + xOffset, curserArrow.transform.position.y + yOffset, curserArrow.transform.position.z);

                if (PopUp)
                {
                    if(Index)
                    {
                        QuestManager.Instance.CreateConfirmationWindow(PopupText, QuestManager.Instance.IncreaseWindowIndex, TopIndex, CurrentIndex);
                    }
                    else
                    {
                        QuestManager.Instance.CreateConfirmationWindow(PopupText, null);
                    }
                }
            }

        }
        catch
        {

        }

    }

    public void ButtonCheck()
    {
        UIElement.GetComponent<Button>().onClick.RemoveListener(ButtonCheck);
        UIElement.GetComponent<Image>().material = null;
        Destroy(curserArrow);
        CompleteQuest();
    }
}
