using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{

    private GameObject CombatUI;
    private PuzzleController PuzzleController;

    //Make an instance
    public static PuzzleManager Instance
    {
        get;

        set;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {            

    }

    private void FindVariablesForPuzzleManager()
    {
        //Potentially needs to be modified for skipping tutorial
        if (GameManager.Instance.CurrentLevel == Levels.Tutorial)
        {
            CombatUI = GameObject.Find("UiManager").transform.Find("Roaming And Combat UI").gameObject;
            PuzzleController = GameObject.Find("PuzzleController").GetComponent<PuzzleController>();
        }
    }

    /// <summary>
    /// This method called from PuzzleRange collison opens the combat UI so the puzzle is possible.
    /// </summary>
    public void OpenPuzzle(GameObject target)
    {
        //Find the variables needed for the project to function
        FindVariablesForPuzzleManager();
        //Open combat UI
        CombatUI.GetComponent<RoamingAndCombatUiController>().SwitchMode(true);
        PuzzleController.Target = target;
    }

    /// <summary>
    /// This method called from PuzzleRange collison closes the combat UI so the puzzle is impossble
    /// removed parameters
    /// </summary>
    public void ClosePuzzle()
    {
        //Switch ui screen
        UiManager.Instance.SwichScreenPuzzle(UiManager.Instance.RoamingAndCombatUI);
        //Switch combat mode to in combat
        CombatUI.GetComponent<RoamingAndCombatUiController>().SwitchMode(false);
        //Set target to null so that the target is cleared
        PuzzleController.Target = null;

        //Start a coroutine to reset the player deck
        StartCoroutine(ChipManager.Instance.PuzzleResetDeck());
    }
}
