using UnityEngine;

public class GameOverUIController : UiController
{
    public Animator Animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Initialize()
    {
        Debug.Log("GameOver Ui initialized");        
    }
    /// <summary>
    /// Is called by Animator
    /// </summary>
    private void OnGameOverAnimationComplete()
    {
        GameManager.Instance.EndCombat();

        // for now just restart the scene.
        GameManager.Instance.RequestScene(Levels.Credits);
    }
}
