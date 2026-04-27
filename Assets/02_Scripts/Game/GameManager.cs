using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // GameManager 오브젝트를 전역으로, 단 하나만 존재함을 보장받고 접근할 수 있게 해주는
    // Instance 변수입니다. 자세한 내용은 '싱글톤(Singleton) 패턴'에 대해 검색해보세요!
    public static GameManager Instance { get; private set; }

    [Tooltip("Event")]
    public event Action IsGameStateBeforeGateOpen;
    public event Action OnGameOver;
    public event Action OnGameClear;

    private const int MAX_WAVE = 9; // Phase 3 보스 = 마지막 웨이브

    private GameFlowState currentState;
    private GameAge currentAge;

    // 웨이브를 만들어 "웨이브당 추가 골드 가중치"와 "몬스터 추가 공격력 및 체력", "세대변화"를 추가하면 될 것같습니다.
    public int wave = 0;
    public int Wave
    {
        get
        {
            return wave;
        }

        set
        {
            wave = value;
            if (wave == 0) return; // 0일 때 0 % 3 == 0으로 보스 페이즈가 되는 것을 방지

            // 3 웨이브 당 1 페이즈로 만들었습니다.
            // 3의 나머지를 이용하여서 만들었습니다.[kwj]
            if (wave % 3 == 1)
            {
                CurrentState = GameFlowState.NORMAL_MONSTER_SPAWN;
            }
            else if (wave % 3 == 2) {
                CurrentState = GameFlowState.ELITE_MONSTER_SPAWN;
            }
            else if (wave % 3 == 0) {
                CurrentState = GameFlowState.BOSS_MONSTER_SPAWN;
            }

            Debug.Log($"{currentState}, {wave}");
        }
    }
    
    // phase 변수를 수동으로 올리지 않고, wave 값에 따라 자동 계산되도록 프로퍼티화 (싱크 꼬임 완벽 해결)
    public int Phase
    {
        get
        {
            return (wave == 0) ? 1 : ((wave - 1) / 3) + 1;
        }
    }

    // 몬스터 마릿수 추적 및 클리어 대기 상태 변수
    public int aliveMonsterCount = 0;
    public bool IsWaitingForClear = false;

    public void AddMonster()
    {
        aliveMonsterCount++;
    }

    public void RemoveMonster()
    {
        aliveMonsterCount--;
        if (aliveMonsterCount <= 0)
        {
            aliveMonsterCount = 0;
            TryAdvanceAfterBoss();
        }
    }

    // IsWaitingForClear 상태이고 살아있는 몬스터가 없을 때 다음 페이즈로 전환.
    // RemoveMonster와 스포너 양쪽에서 호출해 타이밍 경쟁 조건을 방지한다.
    public void TryAdvanceAfterBoss()
    {
        if (!IsWaitingForClear || aliveMonsterCount > 0) return;
        IsWaitingForClear = false;

        if (wave >= MAX_WAVE)
        {
            CurrentState = GameFlowState.GAME_CLEAR;
            return;
        }

        // 현재 Phase 완료 → 다음 시대로 전환 후 준비 단계로
        switch (Phase)
        {
            case 1: CurrentAge = GameAge.MODERN_AGE; break;
            case 2: CurrentAge = GameAge.FUTURE_AGE;  break;
        }

        CurrentState = GameFlowState.BEFORE_GATE_OPEN;
    }
    
    // 외부 스크립트에서 currentState를 참조하고 싶을 때는, 프로퍼티인 CurrentState를 참조하시면 됩니다.
    // 예시: EnemySpawner에서 일반 몬스터의 수가 0이 되었을 경우에
    // GameManager.Instance.CurrentState = GameFlowState.BOSS_MONSTER_SPAWN
    // 이런 식으로 보스 몬스터 페이즈로 넘어가는 로직을 작성할 수 있습니다.
    // currentState를 조건부로 set하는 로직도 해당 프로퍼티에서 작성하시면 됩니다.
    public GameFlowState CurrentState
    {
        get
        {
            return currentState;
        }

        set
        {
            currentState = value;
            
            if (currentState == GameFlowState.BEFORE_GATE_OPEN)
            {
                IsGameStateBeforeGateOpen?.Invoke();
            }

            if (currentState == GameFlowState.GAME_OVER)
            {
                OnGameOver?.Invoke();
            }

            if (currentState == GameFlowState.GAME_CLEAR)
            {
                OnGameClear?.Invoke();
            }

            Debug.Log($"{currentState}, {wave}");
        }
    }

    public GameAge CurrentAge
    {
        get
        {
            return currentAge;
        }

        set
        {
            currentAge = value;
            Debug.Log($"{CurrentAge}");
        }
    }

    void Awake()
    {
        // 31 ~ 38 라인도 싱글톤 패턴과 관련 있는 코드입니다.
        // GameManager가 해당 오브젝트 하나만 존재하도록 하고, 씬이 변경되어도 Destroy되지 않도록 합니다.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // 게임을 완전히 처음 시작할 때는 일단 GAME_START 상태로 시작합니다.
        CurrentState = GameFlowState.GAME_START;

        // 게임을 완전히 처음 시작할 때는 일단 MIDDLE_AGE 상태로 시작합니다.
        CurrentAge = GameAge.MIDDLE_AGE;
    }

    // 게임을 처음부터 다시 시작하고 씬을 초기화할 때 호출하는 메서드입니다.
    // (UI 버튼 이벤트나 게임 오버 재시작 로직에서 호출하세요)
    public void RestartGame()
    {
        // 1. 게임 매니저 변수 초기화
        wave = 0;
        aliveMonsterCount = 0;
        IsWaitingForClear = false;
        CurrentAge = GameAge.MIDDLE_AGE;
        CurrentState = GameFlowState.GAME_START;

        // GoldManager의 골드 초기화 로직 추가
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.ResetGold();
        }

        // 2. 현재 활성화된 씬을 다시 로드하여 맵, 포탑, 몬스터 등을 모두 삭제하고 처음 상태로 되돌림
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드가 완료된 후 상태가 GAME_START라면, 다음 페이즈로 넘어가게 만들었습니다
        // (DontDestroyOnLoad로 살아남은 GameManager는 재시작 시 Start()가 다시 호출되지 않아서)
        if (currentState == GameFlowState.GAME_START)
        {
            CurrentState = GameFlowState.BEFORE_GATE_OPEN;
        }
    }

    void Start()
    {
        if (currentState == GameFlowState.GAME_START)
        {
            // ...

            // Start에서 GAME_START 페이즈에 대한 처리를 끝내고 나면,
            // BEFORE_GATE_OPEN 페이즈로 넘어갑니다. 본격적으로 게임을 시작합니다.
            // 골드 사용, 포탑 배치 등등 ...
            CurrentState = GameFlowState.BEFORE_GATE_OPEN;
        }
    }

    void Update()
    {
        /*
        // 3 웨이브 당 1 페이즈로 만들었습니다.
        // 3의 나머지를 이용하여서 만들었습니다.
        // 자동적으로 웨이브는 증가하도록 만들었고, 웨이브 가중치는 증가하면서 currentState도 변경되도록 만들었습니다.
        if (wave % 3 == 1)
        {
            currentState = GameFlowState.NORMAL_MONSTER_SPAWN;
        }
        else if (wave % 3 == 2) {
            currentState = GameFlowState.ELITE_MONSTER_SPAWN;
        }
        else if (wave % 3 == 0) {
            currentState = GameFlowState.BOSS_MONSTER_SPAWN;
        }
        */

        // 리팩터링 ::
        // Update에서 계속 currentState를 갱신하게 하는 방법보다, wave 값이 바뀔 때만 currentState를 갱신하도록 만들었습니다.
        // 해당 내용 wave 변수를 포커싱하는 Wave 프로퍼티를 만들어서 set 블록에 구현하였습니다!! 참고해주세요

        if (currentState == GameFlowState.BEFORE_GATE_OPEN)
        {
            // ...
        }

        if (currentState == GameFlowState.NORMAL_MONSTER_SPAWN)
        {
            // ...
        }

        if (currentState == GameFlowState.BOSS_MONSTER_SPAWN)
        {
            // ...
        }

        if (currentState == GameFlowState.GAME_OVER)
        {
            // ...
        }
    }
}
