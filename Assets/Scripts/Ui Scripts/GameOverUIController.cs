using UnityEngine;

public class GameOverUIController : UiController
{
    public Animator Animator;

    [Header("Sounds")]

    public SoundFX GameOverSound;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.PlayFXSound(GameOverSound);
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
