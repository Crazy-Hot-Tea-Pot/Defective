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
                //Get the image component and apply our material
                UIElement.GetComponent<Image>().material = mat;

                //Add a button component
                UIElement.GetComponent<Button>().onClick.RemoveListener(ButtonCheck);
                UIElement.GetComponent<Button>().onClick.AddListener(ButtonCheck);
            }

            if (GameObject.Find(UIElementPath).transform.Find(curserArrow.name) == null)
            {
                curserArrow = Instantiate(curserArrow, UIElement.transform.parent.Find(UIElement.name));
                curserArrow.transform.position = new Vector3(curserArrow.transform.position.x + xOffset, curserArrow.transform.position.y + yOffset, curserArrow.transform.position.z);

                QuestManager.Instance.CreateConfirmationWindow("You can access quest by following the arrow above", null);
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
