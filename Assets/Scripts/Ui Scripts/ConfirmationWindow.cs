using System;
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
    [SerializeField]
    private Action onConfrimAction;


    void OnEnable()
    {
        cancelButton.onClick.AddListener(DestroyWindow);        
        ConfirmationButton.onClick.AddListener(InvokeConfirmAction);        
    }
    public void SetUpComfirmationWindow(string Message, Action onConfirm)
    {
        TextBox.SetText(Message);
        onConfrimAction = onConfirm;
    }

    private void InvokeConfirmAction()
    {
        onConfrimAction?.Invoke();
        DestroyWindow();
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
