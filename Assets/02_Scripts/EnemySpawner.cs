using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject normalPrefab;
    public GameObject elitePrefab;
    public GameObject bossPrefab;

    public Transform spawnPoint;
    public GameFlowState currentState; //현재 게임 단계 (테스트용입니다 나중에는 GameManager에서 받아오면 될 것 같습니다)

    void Start()
    {
        if (spawnPoint == null) spawnPoint = transform; //만약 스폰 지점이 없으면 현재 게이트 위치를 기본값으로 사용합니다
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스바를 누르면 즉시 스폰됩니다 (테스트용입니다)
        {
            SpawnByState();
        }

        if (Input.GetKeyDown(KeyCode.T)) // T키를 누르면 2초 간격으로 5마리씩 나오는 웨이브가 시작됩니다 (테스트용입니다)
        {
            StartCoroutine(StartWave(5, 2.0f, 1));
        }
    }

    /* 실제 개발할때는 Update 말고 아래 함수처럼 이벤트 형식으로 하는게 좋을 것 같습니다
    public void OnWaveStart()
    {
        StartCoroutine(StartWave(10, 2.0f,1));
    } */

    public void SpawnByState() //현재 단계에 맞춰서 어떤 몹을 소환할지 결정합니다
    {
        GameObject selectedPrefab = null;

        switch (currentState)
        {
            case GameFlowState.NORMAL_MONSTER_SPAWN:
                selectedPrefab = normalPrefab;
                break;

            case GameFlowState.ELITE_MONSTER_SPAWN:
                selectedPrefab = elitePrefab;
                break;

            case GameFlowState.BOSS_MONSTER_SPAWN:
                selectedPrefab = bossPrefab;
                break;
        }

        if (selectedPrefab != null)
        {
            //어떤 몹을, 어디에, 어떤 각도로 생성할지 정하는 생성 함수입니다 
            GameObject obj = Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);
            Monster monster = obj.GetComponent<Monster>();

            if (monster != null) //프리팹 연결을 깜빡할 경우를 대비해 예외처리 했습니다
            {
                monster.Init();
            }
        }
        
    }
    
    
    //count = 웨이브 횟수, interval = 다음 소환까지 대기 시간, amountPerTime = 한번에 나올 몬스터 수
    public IEnumerator StartWave(int count, float interval, int amountPerTime)
    {
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < amountPerTime; j++)
            {
                SpawnByState(); //몬스터를 생성합니다
            }

            yield return new WaitForSeconds(interval); // interval초만큼 대기
        }
    }
}
