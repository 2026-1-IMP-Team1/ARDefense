using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainUIManager : MonoBehaviour
{
    //MainUI와 OptionUI를 연결하기 위한 변수
    public GameObject MainUI;
    public GameObject OptionUI;

    // 시작시에 MainUI를 활성화하고 OptionUI를 비활성화하여 초기 상태를 설정
    void Start()
    {
        MainUI.SetActive(true);
        OptionUI.SetActive(false);
    }
    // 옵션 UI를 열 때 MainUI를 비활성화하고 OptionUI를 활성화하는 메서드
    public void OpenOptionUI()
    {
        MainUI.SetActive(false);
        OptionUI.SetActive(true);
    }
    // 옵션 UI를 닫을 때 MainUI를 활성화하고 OptionUI를 비활성화하는 메서드
    public void CloseOptionUI()
    {
        MainUI.SetActive(true);
        OptionUI.SetActive(false);
    }
    // 게임을 종료하는 메서드
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    // 볼륨을 조절하는 메서드
    public void VolumeControl(float volume)
    {
        Debug.Log("Volume: " + volume);
        AudioListener.volume = volume;
    }

    public void GoToGameUI()
    {
        // 게임 UI로 전환하는 로직을 여기에 추가하세요.
        SceneManager.LoadScene("MainGameScene");
    }
}
