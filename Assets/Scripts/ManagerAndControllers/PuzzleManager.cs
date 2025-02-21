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
        //Potentially needs to be modified for skipping tutorial
        if (GameManager.Instance.CurrentLevel == Levels.Tutorial)
        {
            //It is possible to load this script before this object exits
            try
            {
                CombatUI = GameObject.Find("UiManager").transform.Find("Roaming And Combat UI").gameObject;
                PuzzleController = GameObject.Find("PuzzleController").GetComponent<PuzzleController>();
            }
            catch
            {

            }
        }
    }

    /// <summary>
    /// This method called from PuzzleRange collison opens the combat UI so the puzzle is possible.
    /// </summary>
    public void OpenPuzzle(GameObject target)
    {
        Debug.Log("Opened");
        //Open combat UI
        CombatUI.GetComponent<RoamingAndCombatUiController>().SwitchMode(true);
        PuzzleController.Target = target;
    }

    /// <summary>
    /// This method called from PuzzleRange collison closes the combat UI so the puzzle is impossble
    /// </summary>
    public void ClosePuzzle(GameObject target)
    {
        Debug.Log("Cosed");
        //Close combat UI

        CombatUI.GetComponent<RoamingAndCombatUiController>().SwitchMode(false);
        PuzzleController.Target = target;
    }
}
