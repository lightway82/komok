using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    
    [SerializeField]
    private Button DialogButton1;
    
    [SerializeField]
    private Text Button1Text;
    
    [SerializeField]
    private Button DialogButton2;
    
    [SerializeField]
    private Text Button2Text;
    
    [SerializeField]
    private Text DialogText;

    private Action btn1Action;//экшены кнопок, их меняет вызывающий диалог код
    private Action btn2Action;


    private void Awake()
    {
        DialogButton1.onClick.AddListener(()=>btn1Action());
        DialogButton2.onClick.AddListener(()=>btn2Action());
    }

    /// <summary>
    /// иНФОРМАЦИОННЫЙ диалог
    /// </summary>
    /// <param name="text"></param>
    /// <param name="buttonText"></param>
    public void OpenInfoDialog(string text, string buttonText)
    {
        Button1Text.text = buttonText;
        DialogButton1.gameObject.SetActive(true);
        DialogButton2.gameObject.SetActive(false);
        DialogText.text = text;
        gameObject.SetActive(true);
        btn1Action = CloseDialogPanel;

    }
    
    /// <summary>
    /// Диалог выбора действий
    /// </summary>
    /// <param name="text"></param>
    /// <param name="buttonText1"></param>
    /// <param name="buttonText2"></param>
    /// <param name="btn1Action"></param>
    /// <param name="btn2Action"></param>
    public void OpenChoiceDialog(string text, string buttonText1, string buttonText2, Action btn1Action, Action btn2Action)
    {
        Button1Text.text = buttonText1;
        Button2Text.text = buttonText2;
        DialogButton1.gameObject.SetActive(true);
        DialogButton2.gameObject.SetActive(true);
        DialogText.text = text;
        this.btn1Action = btn1Action;
        this.btn2Action = btn2Action;
        gameObject.SetActive(true);
    }
    
    public void CloseDialogPanel()
    {
        gameObject.SetActive(false);
        
    }
}
