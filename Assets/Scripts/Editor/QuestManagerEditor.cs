using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class QuestManagerEditor : EditorWindow
{
    //A list of scriptable options for quests
    public string[] optionsList = {"Go Here", "Highlight UI"};
    //Bool drop down
    bool dropDown;

    //Checks for different UI
    bool GoHereIsOpen = false;
    bool QuestUIHighlighIsOpen = false;

    //Variables for input
    string fileName = "Blank File";
    string description = "Blank Description";
    string qName = "Blank Name";

    //Quest Path
    string questPath = " ";
    string materialPath = " ";

    /// <summary>
    /// This ceates the window in the toolbar
    /// </summary>
    [MenuItem("Tools/Quest Creation Tool")]
    public static void OpenEditor()
    {
        //This reates an editor window variable from the get of the window that is created from this editor window
        EditorWindow editWind = GetWindow<QuestManagerEditor>();
        //Now we can name the window so it will show up in the GUI
        editWind.titleContent = new GUIContent("Quest Creation Tool");
    }

    public void OnGUI()
    {
        RunDropDown();
        openGoHereUI();
    }

    /// <summary>
    /// Holds the UI for drop down that I had to invent thank you Unity for not including it
    /// </summary>
    public void RunDropDown()
    {
        if (dropDown == true)
        {
            if (GUILayout.Button("Close Quest List"))
            {
                dropDown = false;
            }

            GUILayout.Label("\n\n\n");
            if (GUILayout.Button("Go Here Quest"))
            {
                GoHereIsOpen = true;
                QuestUIHighlighIsOpen = false;
                dropDown = false;
            }

            GUILayout.Label("\n\n\n");
            if(GUILayout.Button("Quest UI Highlight"))
            {
                QuestUIHighlighIsOpen = true;
                GoHereIsOpen = false;
                dropDown = false;
            }

        }
        else if (dropDown == false)
        {
            if (GUILayout.Button("Select Quest V"))
            {
                dropDown = true;
                GoHereIsOpen = false;
            }
        }
    }

    /// <summary>
    /// The UI and function control for the GO Here quests
    /// </summary>
    public void openGoHereUI()
    {
        if(GoHereIsOpen)
        {
            GUILayout.Label("Quest File Name");
            fileName = EditorGUILayout.TextArea(fileName, GUILayout.Height(50));
            GUILayout.Label("\n\n\n\nQuest Name");
            qName = EditorGUILayout.TextArea(qName, GUILayout.Height(50));
            GUILayout.Label("\n\n\n\nQuest Description");
            description = EditorGUILayout.TextArea(description, GUILayout.Height(50));

            GUILayout.Label("\n\n\n\nPlace a quest marker for the player to interact with");
            if (GUILayout.Button("Place Marker"))
            {
                QuestGoHere currentQuest = null;
                try
                {
                    AssetDatabase.CreateAsset(currentQuest = ScriptableObject.CreateInstance<QuestGoHere>(), "Assets/Resources/Scriptables/Quest/" + fileName + ".asset");
                    currentQuest.questName = qName;
                    currentQuest.questDesc = description;
                    GameObject questMarker = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Ui Prefabs/QuestObjects/QuestMarker.prefab", typeof(GameObject));
                    currentQuest.questEndNameForPositionCalc = Instantiate(questMarker).name;
                    EditorUtility.SetDirty(currentQuest);
                }
                catch
                {
                    Debug.Log("Doesn't Exist");
                }
            }
        }
        else if(QuestUIHighlighIsOpen)
        {
            GUILayout.Label("Quest File Name");
            fileName = EditorGUILayout.TextArea(fileName, GUILayout.Height(50));
            GUILayout.Label("\nQuest Name");
            qName = EditorGUILayout.TextArea(qName, GUILayout.Height(50));
            GUILayout.Label("\nQuest Description");
            description = EditorGUILayout.TextArea(description, GUILayout.Height(50));
            GUILayout.Label("\nQuest Path");
            GUILayout.Label("UiManager/Roaming And Combat UI/");
            GUILayout.Label("You can't just directly call for example MiniLog you need /MiniBarSettingsAndUi/MiniLog");
            questPath = EditorGUILayout.TextArea(questPath, GUILayout.Height(50));
            GUILayout.Label("\nMaterial Name");
            materialPath = EditorGUILayout.TextField(materialPath, GUILayout.Height(50));

            if (GUILayout.Button("Generate UI Highlight"))
            {
                QuestUIHighlight currentQuest = null;
                try
                {
                    AssetDatabase.CreateAsset(currentQuest = ScriptableObject.CreateInstance<QuestUIHighlight>(), "Assets/Resources/Scriptables/Quest/" + fileName + ".asset");
                    currentQuest.questName = qName;
                    currentQuest.questDesc = description;
                    currentQuest.UIElementPath = "UiManager/Roaming And Combat UI/" + questPath;
                    EditorUtility.SetDirty(currentQuest);
                }
                catch
                {
                    Debug.Log("Doesn't Exist");
                }
            }
        }
    }

}
