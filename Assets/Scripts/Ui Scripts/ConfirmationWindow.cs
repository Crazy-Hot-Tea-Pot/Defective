using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationWindow : MonoBehaviour
{
    public Button ConfirmationButton
    {
        get
        {
            return confirmationButton;
        }
        set
        {
            confirmationButton = value;
        }
    }

    [SerializeField]
    private TextMeshProUGUI TextBox;
    [SerializeField]
    private Button confirmationButton;
    [SerializeField]
    private Button cancelButton;
    void OnEnable()
    {
        cancelButton.onClick.AddListener(DestroyWindow);
        ConfirmationButton.onClick.AddListener(DestroyWindow);
        
    }
    public void SetUpComfirmationWindow(string Message)
    {
        TextBox.SetText(Message);
    }
    private void DestroyWindow()
    {
        Destroy(this.gameObject);
    }
    void OnDestroy()
    {
        cancelButton?.onClick.RemoveListener(DestroyWindow);
        ConfirmationButton.onClick.RemoveAllListeners();
    }
}
