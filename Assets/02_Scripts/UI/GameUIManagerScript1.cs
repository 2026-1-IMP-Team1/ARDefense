using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameUIManager : MonoBehaviour
{
    //MainUI와 OptionUI를 연결하기 위한 변수
    public GameObject MainUI;
    public GameObject OptionUI;

    //골드 매니저에서 골드 값을 가져와서 UI에 표시하기 위한 변수
    //public int Gold = GoldManager.Instance.Gold;
    public int Gold = 0;
    public TextMeshProUGUI GoldText;

    //시간 매니저에서 시간 값을 가져와서 UI에 표시하기 위한 변수
    public TextMeshProUGUI minText;
    public TextMeshProUGUI secText;
    private float elapsedTime = 0f;

    // 터렛 설치를 위한 UI 변수
    public GameObject PlantUI;

    // 시작시에 MainUI를 활성화하고 OptionUI를 비활성화하여 초기 상태를 설정
    void Start()
    {
        MainUI.SetActive(true);
        OptionUI.SetActive(false);
        PlantUI.SetActive(false);

        // 자꾸 Awake랑 Enable이랑 GoldManager 싱글톤 시점이 어중간해서 Start에서 구독하게 했습니다.
        GoldManager.Instance.OnGoldChanged += GoldNumManager;
    }

    void OnDisable()
    {
        GoldManager.Instance.OnGoldChanged -= GoldNumManager;
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

    // 골드 값을 UI에 표시하는 메서드
    public void GoldNumManager()
    {
        GoldText.text = $"{GoldManager.Instance.Gold}";
    }

    // 경과 시간을 계산하여 분과 초로 나누어 UI에 표시하는 메서드
    public void TimeNumManager()
    {
        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        minText.text = string.Format("{0:00}", minutes);
        secText.text = string.Format("{0:00}", seconds);
    }
    
    // 메인매뉴로 돌아가는 메서드
    public void GoToMainUI()
    {
        SceneManager.LoadScene("MainUI");
    }

    public void OpenPlantUI()
    {
        PlantUI.SetActive(true);
    }

    // 매 프레임마다 골드 값과 경과 시간을 업데이트하는 메서드
    void Update()
    {
        GoldNumManager();
        TimeNumManager();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("PlantSpot"))
                {
                    OpenPlantUI();
                }
                else
                {
                    PlantUI.SetActive(false);
                }
            } else
            {
                PlantUI.SetActive(false);
            }
        }
    }
}
