using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "QuestUIHighlight", menuName = "Quest/QuestUIHighlight")]
public class QuestUIHighlight : Quest
{
    public Material mat;
    [Header("This will find and utalize a button")]
    public string UIElementPath;
    private GameObject UIElement = null;

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
            UIElement = GameObject.Find(UIElementPath);
            //Debug.Log(UIElement);

            //If we have a button to change
            if (UIElement.GetComponent<Button>() == true)
            {
                //Get the image component and apply our material
                UIElement.GetComponent<Image>().material = mat;

                //Add a button component
                UIElement.GetComponent<Button>().onClick.RemoveListener(ButtonCheck);
                UIElement.GetComponent<Button>().onClick.AddListener(ButtonCheck);
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
        CompleteQuest();
    }
}
