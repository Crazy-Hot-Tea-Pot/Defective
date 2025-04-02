using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TrailerExitHandler : MonoBehaviour
{
    private PlayerInputActions input;    
    private InputAction anyInput;
    private InputAction anyMouseAction;
    private float inputDelay = 0.5f;
    private float timer = 0f;

    void Awake()
    {
        input = new PlayerInputActions();        
        anyInput = input.UI.AnyKeyboardInput; 
        anyMouseAction=input.UI.AnyMouseInput;
        anyInput.Enable();
        anyMouseAction.Enable();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > inputDelay && (anyInput.triggered || anyMouseAction.triggered))
        {
            SceneManager.LoadScene("Title");
            //SceneManager.UnloadSceneAsync("Trailer");
        }
    }

    void OnDestroy()
    {        
        anyInput.Disable();
        anyMouseAction.Disable();
    }
}
