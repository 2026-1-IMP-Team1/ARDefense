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
        // 보스 스폰 후 남은 몬스터들이 전멸할 때까지 스폰 로직 대기
        if (GameManager.Instance.IsWaitingForClear) return;

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

        // amountPerTime에 따라 몬스터를 소환하게 만들었습니다(원래는 5로 되있어서 수정)(kwj)
        for (int i = 0; i < amountPerTime; i++)
        {
            // 게이트 문 앞으로 0.1m 지점에서 생성
            Vector3 monsterPos = spawnedGate.transform.position + (spawnedGate.transform.forward * 0.1f) + new Vector3(0, 0.1f, 0);
            
            // enemyPrefab을 사용하여 소환
            Instantiate(enemyPrefab, monsterPos, spawnedGate.transform.rotation);
            
            yield return new WaitForSeconds(0.8f);
        }

        if (GameManager.Instance.CurrentState == GameFlowState.BOSS_MONSTER_SPAWN)
        {
            // 보스 스폰 직후 wave 증가 및 클리어 대기 상태 돌입
            GameManager.Instance.IsWaitingForClear = true;
        }
        else
        {
            GameManager.Instance.Wave++;
        }
        hasStartedWave = false; // 다음 웨이브 진행을 위해 초기화
    }
}