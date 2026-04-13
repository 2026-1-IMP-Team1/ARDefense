using UnityEngine;

public class GameManager : MonoBehaviour
{
    // GameManager 오브젝트를 전역으로, 단 하나만 존재함을 보장받고 접근할 수 있게 해주는
    // Instance 변수입니다. 자세한 내용은 '싱글톤(Singleton) 패턴'에 대해 검색해보세요!
    public static GameManager Instance { get; private set; }

    private GameFlowState currentState;

    // 웨이브를 만들어 "웨이브당 추가 골드 가중치"와 "몬스터 추가 공격력 및 체력", "세대변화"를 추가하면 될 것같습니다.
    private int wave = 0;
    public int Wave
    {
        get
        {
            return wave;
        }

        set
        {
            wave = value;
        }
    }

    private int weight = 3;
    public int WaveWeight => weight * wave;
    
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
            currentState = GameFlowState.BEFORE_GATE_OPEN;
        }
    }

    void Update()
    {
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

