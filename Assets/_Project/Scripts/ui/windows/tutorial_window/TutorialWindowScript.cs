using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialWindowScript : WindowScript
{
    public static TutorialWindowScript instance;

    public GameObject Character;
    public GameObject Boxchat;
    public Text TutorialText;
    public Button ContinueButton;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (ContinueButton != null)
        {
            ContinueButton.onClick.AddListener(OnClickContinueButton);
        }
    }

    public void SetTutorialContent(string content)
    {
        if (TutorialText != null)
        {
            TutorialText.text = content;
        }
    }

    public void ShowCharacter(bool show)
    {
        if (Character != null)
        {
            Character.SetActive(show);
        }
    }

    public void ShowBoxchat(bool show)
    {
        if (Boxchat != null)
        {
            Boxchat.SetActive(show);
        }
    }

    public void OnClickContinueButton()
    {
        Debug.Log("Tutorial Continue Button Clicked");
        this.Close();
    }

    public override void Close()
    {
        base.Close();
    }
}
