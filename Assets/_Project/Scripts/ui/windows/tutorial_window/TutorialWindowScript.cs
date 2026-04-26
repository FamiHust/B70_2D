using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class TutorialStep
{
    public string text;
    public Sprite characterSprite;
}

public class TutorialWindowScript : WindowScript
{
    public static TutorialWindowScript instance;

    public GameObject Character;
    public Image CharacterImage;
    public GameObject Boxchat;
    public Text TutorialText;
    public Button ContinueButton;
    public Animator anim;

    public List<TutorialStep> TutorialSteps = new List<TutorialStep>();
    private int _currentTextIndex = 0;

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
        this.ShowCurrentStep();
    }

    public void SetTutorialContent(string content)
    {
        if (TutorialText != null)
        {
            TutorialText.text = content;
        }
    }

    private void ShowCurrentStep()
    {
        if (TutorialSteps != null && _currentTextIndex < TutorialSteps.Count)
        {
            TutorialStep step = TutorialSteps[_currentTextIndex];
            
            this.SetTutorialContent(step.text);

            if (CharacterImage != null && step.characterSprite != null)
            {
                CharacterImage.sprite = step.characterSprite;
            }

            // this.ShowCharacter(true);
        }
    }


    public void PlayChatAnim()
    {
        if (Boxchat != null)
        {
            Boxchat.transform.DOKill();
            Boxchat.transform.localScale = Vector3.one;
            Boxchat.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f);
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
        _currentTextIndex++;
        if (TutorialSteps == null || _currentTextIndex >= TutorialSteps.Count)
        {
            Debug.Log("Tutorial Finished - Moving to GameOverlay Step");
            
            if (GameOverlayWindowScript.instance != null)
            {
                GameOverlayWindowScript.instance.ShowOverlay();
                GameOverlayWindowScript.instance.SetTutorialState(true);
                GameOverlayWindowScript.instance.transform.SetAsLastSibling();
            }

            // Do not close TutorialWindow and keep elements visible
            if (ContinueButton != null) ContinueButton.gameObject.SetActive(false);
        }
        else
        {
            this.ShowCurrentStep();
        }
    }

    public override void Close()
    {
        this.HideWindow();

        // Restore normal state of GameOverlay
        if (GameOverlayWindowScript.instance != null)
        {
            GameOverlayWindowScript.instance.SetTutorialState(false);
        }

        base.Close();
    }

    public void HideWindow()
    {
        if (anim != null) anim.Play("Hide");
    }

    public void ShowWindow()
    {
        if (anim != null) anim.Play("Show");
    }
}
