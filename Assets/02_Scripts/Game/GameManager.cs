using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // GameManager 오브젝트를 전역으로, 단 하나만 존재함을 보장받고 접근할 수 있게 해주는
    // Instance 변수입니다. 자세한 내용은 '싱글톤(Singleton) 패턴'에 대해 검색해보세요!
    public static GameManager Instance { get; private set; }

    [Tooltip("Event")]
    public event Action IsGameStateBeforeGateOpen;

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
        }
    }
    public int phase;
    
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
        currentState = GameFlowState.GAME_START;

        // 게임을 완전히 처음 시작할 때는 일단 MIDDLE_AGE 상태로 시작합니다.
        currentAge = GameAge.MIDDLE_AGE;
    }

    void OnEnable()
    {
        // ...
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
