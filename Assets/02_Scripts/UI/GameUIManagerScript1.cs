using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameUIManager : MonoBehaviour
{
    // Variables to link MainUI and OptionUI
    public GameObject MainUI;
    public GameObject OptionUI;

    // Variables to get the gold value from GoldManager and display it on the UI
    //public int Gold = GoldManager.Instance.Gold;
    public int Gold = 0;
    public TextMeshProUGUI GoldText;

    // Variables to display the elapsed time on the UI
    public TextMeshProUGUI minText;
    public TextMeshProUGUI secText;
    private float elapsedTime = 0f;

    // UI variable for turret installation
    public GameObject PlantUI;

    // UI for user preparation phase (BEFORE_GATE_OPEN) with a start button
    public GameObject phaseReadyUI;
    public Button readyButton;

    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameClearUI;
    [SerializeField] private GameObject TurretSelectUI;
    [SerializeField] private TextMeshProUGUI ageText;
    [SerializeField] private TextMeshProUGUI insufficientGoldText;
    private Coroutine insufficientGoldCoroutine;

    [SerializeField] private TurretInstaller turretInstaller;
    [SerializeField] private TextMeshProUGUI alreadyInstalledText;
    private Coroutine alreadyInstalledCoroutine;

    // Sets the initial state by activating MainUI and deactivating OptionUI at start
    void Start()
    {
        MainUI.SetActive(true);
        OptionUI.SetActive(false);
        PlantUI.SetActive(false);
        phaseReadyUI.SetActive(false);

        // Subscribing to events in Start to ensure Singleton instances are fully initialized
        GoldManager.Instance.OnGoldChanged += GoldNumManager;
        GameManager.Instance.IsGameStateBeforeGateOpen += ShowPhaseReadyUI;
        GameManager.Instance.OnGameOver += ShowGameOverUI;
        GameManager.Instance.OnGameClear += ShowGameClearUI;
        GameManager.Instance.OnAgeChanged += UpdateAgeText;
        GoldManager.Instance.OnGoldInsufficient += ShowInsufficientGoldText;
    }

    void OnEnable()
    {
        // Subscribe to ready button click event
        readyButton.onClick.AddListener(OnReadyButtonClicked);
    }

    void OnDisable()
    {
        GoldManager.Instance.OnGoldChanged -= GoldNumManager;
        GameManager.Instance.IsGameStateBeforeGateOpen -= ShowPhaseReadyUI;
        GameManager.Instance.OnGameOver -= ShowGameOverUI;
        GameManager.Instance.OnGameClear -= ShowGameClearUI;
        GameManager.Instance.OnAgeChanged -= UpdateAgeText;
        readyButton.onClick.RemoveListener(OnReadyButtonClicked);
        GoldManager.Instance.OnGoldInsufficient -= ShowInsufficientGoldText;
    }

    // Method to open the option UI
    public void OpenOptionUI()
    {
        MainUI.SetActive(false);
        OptionUI.SetActive(true);
    }

    // Method to close the option UI
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
    // Method to control the audio volume
    public void VolumeControl(float volume)
    {
        Debug.Log("Volume: " + volume);
        AudioListener.volume = volume;
    }

    // Method to display the current gold value on the UI
    public void GoldNumManager()
    {
        GoldText.text = $"{GoldManager.Instance.Gold}";
    }
    
    // Method to calculate elapsed time and display it as minutes and seconds
    public void TimeNumManager()
    {
        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        minText.text = string.Format("{0:00}", minutes);
        secText.text = string.Format("{0:00}", seconds);
    }
    
    // Method to return to the main menu scene
    public void GoToMainUI()
    {
        GameManager.Instance.RestartGame();
        SceneManager.LoadScene("MainUI");
    }

    // Method to open the turret planting UI
    public void OpenPlantUI()
    {
        PlantUI.SetActive(true);
    }

    // Method to update gold and time every frame
    
    void Update()
    {
        GoldNumManager();
        TimeNumManager();
    }
    
    // Method to show the phase ready UI
    private void ShowPhaseReadyUI()
    {
        phaseReadyUI.SetActive(true);
    }

    // Method to show the game over UI
    private void ShowGameOverUI()
    {
        gameOverUI?.SetActive(true);
    }

    // Method to show the game clear UI
    private void ShowGameClearUI()
    {
        phaseReadyUI?.SetActive(false);
        gameClearUI?.SetActive(true);
    }

    // Method to update the text displaying the current game age
    private void UpdateAgeText()
    {
        ageText.text = GameManager.Instance.CurrentAge switch
        {
            GameAge.MIDDLE_AGE => "Middle Age",
            GameAge.MODERN_AGE => "Modern Age",
            GameAge.FUTURE_AGE => "Future Age",
            _ => ""
        };
    }

    // Event handler for the ready button click
    private void OnReadyButtonClicked()
    {
        // Debug.Log("NORMAL_MONSTER_SPAWN");
        GameManager.Instance.Wave++; // wave = 1이 되므로, CurrentState = NORMAL_MONSTER_SPAWN;
        phaseReadyUI.SetActive(false);
    }

    // Method to display insufficient gold warning text
    private void ShowInsufficientGoldText()
    {
        if (insufficientGoldCoroutine != null) StopCoroutine(insufficientGoldCoroutine);
        insufficientGoldCoroutine = StartCoroutine(InsufficientGoldRoutine());
    }

    // Coroutine to handle the display duration of the insufficient gold text
    IEnumerator InsufficientGoldRoutine()
    {
        insufficientGoldText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        insufficientGoldText.gameObject.SetActive(false);
    }

    // Method to display warning text when a turret is already installed
    private void ShowAlreadyInstalledText()
    {
        if (alreadyInstalledCoroutine != null) StopCoroutine(alreadyInstalledCoroutine);
        alreadyInstalledCoroutine = StartCoroutine(AlreadyInstalledRoutine());
    }

    // Coroutine to handle the display duration of the already installed text
    IEnumerator AlreadyInstalledRoutine()
    {
        alreadyInstalledText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        alreadyInstalledText.gameObject.SetActive(false);
    }
}
