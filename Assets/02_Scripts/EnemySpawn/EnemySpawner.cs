using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;    
    public GameObject gatePrefab;    

    public GameBoardGenerator boardGenerator;
    public float waitTimeAfterBoard = 3.0f; 

    private bool hasStartedWave = false;
    private GameObject spawnedGate;
    public int amountPerTime;
    


    void Update()
    {
        // 현재 게임 단계에 맞춰서 한번에 나오는 몬스터 수를 결정하는 로직입니다.
        // StartWave함수에서 사용
        if(GameManager.Instance.CurrentState == GameFlowState.NORMAL_MONSTER_SPAWN)
        {
            amountPerTime = MonsterStats.NORMAL_MONSTER_Number;
        }
        else if(GameManager.Instance.CurrentState == GameFlowState.ELITE_MONSTER_SPAWN)
        {
            amountPerTime = MonsterStats.ELITE_MONSTER_Number;
        }
        else if(GameManager.Instance.CurrentState == GameFlowState.BOSS_MONSTER_SPAWN)
        {
            amountPerTime = MonsterStats.BOSS_MONSTER_Number;
        }

        if (!hasStartedWave && boardGenerator != null && boardGenerator.IsGameBoardGenerated)
        {
            if (boardGenerator.GameBoard != null && boardGenerator.GameBoard.transform.childCount > 0)
            {
                if (GameManager.Instance.CurrentState == GameFlowState.BEFORE_GATE_OPEN)
                {
                    // Debug.Log("BEFORE_GATE_OPEN");
                    hasStartedWave = false;
                    return;
                }

                hasStartedWave = true;
                StartCoroutine(SimpleSpawnSequence());
            }
        }
    }

    IEnumerator SimpleSpawnSequence()
    {
        Vector3 boardCenterPos = boardGenerator.GameBoard.transform.position;
        
        GameObject player = GameObject.FindWithTag("Player");
        Vector3 spawnPos = boardCenterPos; 
        Quaternion spawnRot = Quaternion.identity;

        if (player != null)
        {
            Vector3 dirFromPlayer = (boardCenterPos - player.transform.position).normalized;
            dirFromPlayer.y = 0; 

            // 임시로 plane 중앙에서 플레이어 반대 방향으로 0.5m 지점에 게이트 생성했습니다
            spawnPos = boardCenterPos + (dirFromPlayer * 0.5f);
            spawnRot = Quaternion.LookRotation(-dirFromPlayer); 
        }

        spawnedGate = Instantiate(gatePrefab, spawnPos, spawnRot);
        
        GateController gateCtrl = spawnedGate.GetComponent<GateController>();
        if (gateCtrl != null) gateCtrl.OpenGate();

        yield return new WaitForSeconds(waitTimeAfterBoard);

        for (int i = 0; i < 5; i++)
        {
            // 게이트 문 앞으로 0.1m 지점에서 생성
            Vector3 monsterPos = spawnedGate.transform.position + (spawnedGate.transform.forward * 0.1f) + new Vector3(0, 0.1f, 0);
            
            // enemyPrefab을 사용하여 소환
            Instantiate(enemyPrefab, monsterPos, spawnedGate.transform.rotation);
            
            yield return new WaitForSeconds(2.0f);
        }

        // 3번째 보스 페이즈(마지막 페이즈)가 끝났다면 다음 웨이브로 즉시 넘어가지 않고 정비 상태로 변경[kwj]
        if (GameManager.Instance.CurrentState == GameFlowState.BOSS_MONSTER_SPAWN)
        {
            GameManager.Instance.CurrentState = GameFlowState.BEFORE_GATE_OPEN;
        }
        else
        {
            GameManager.Instance.Wave++;
        }
        hasStartedWave = false; // 다음 웨이브 진행을 위해 초기화
    }
}