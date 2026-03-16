using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuWindowScript : WindowScript
{
    public Button PlayButton;

    void Start()
    {
        if (PlayButton != null)
        {
            PlayButton.onClick.AddListener(OnPlayClicked);
        }
    }

    void OnPlayClicked()
    {
        // call SceneManager to enter normal mode
        if (SceneManager.instance != null)
        {
            SceneManager.instance.EnterNormalMode();
        }

        // disable button to avoid repeat clicks
        if (PlayButton != null) PlayButton.interactable = false;

        // wait for SceneEnteringWindow to be created, then close when it invokes its intermediate callback
        StartCoroutine(_WaitAndCloseAfterSceneEntering());
    }

    private System.Collections.IEnumerator _WaitAndCloseAfterSceneEntering()
    {
        SceneEnteringWindowScript sceneWindow = null;
        // wait until the SceneEnteringWindow instance appears
        while (sceneWindow == null)
        {
            sceneWindow = UnityEngine.Object.FindObjectOfType<SceneEnteringWindowScript>();
            yield return null;
        }

        // subscribe to its OnIntermediate event to close menu after transition
        sceneWindow.OnIntermediate += _OnSceneEntered;
    }

    private void _OnSceneEntered()
    {
        // unsubscribe and close
        SceneEnteringWindowScript sceneWindow = UnityEngine.Object.FindObjectOfType<SceneEnteringWindowScript>();
        if (sceneWindow != null)
            sceneWindow.OnIntermediate -= _OnSceneEntered;

        this.Close();
    }
}
