using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainUIManager : MonoBehaviour
{
    // Variables to link MainUI and OptionUI
    public GameObject MainUI;
    public GameObject OptionUI;

    // At the start, activate MainUI and deactivate OptionUI to set the initial state
    void Start()
    {
        MainUI.SetActive(true);
        OptionUI.SetActive(false);
    }
    // Method to deactivate MainUI and activate OptionUI when opening the option UI
    public void OpenOptionUI()
    {
        MainUI.SetActive(false);
        OptionUI.SetActive(true);
    }
    // Method to activate MainUI and deactivate OptionUI when closing the option UI
    public void CloseOptionUI()
    {
        MainUI.SetActive(true);
        OptionUI.SetActive(false);
    }
    // Method to exit the game
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    // Method to control the volume
    public void VolumeControl(float volume)
    {
        Debug.Log("Volume: " + volume);
        AudioListener.volume = volume;
    }

    public void GoToGameUI()
    {
        // Add logic to switch to the game UI here.
        SceneManager.LoadScene("MainGameScene");
    }
}
