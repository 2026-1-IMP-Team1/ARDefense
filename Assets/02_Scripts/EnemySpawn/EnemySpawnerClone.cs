using UnityEngine;
using System.Collections;

public class EnemySpawnerClone : MonoBehaviour
{
    [SerializeField] private GameObject normalMonsterPrefab;
    [SerializeField] private GameObject eliteMonsterPrefab;
    [SerializeField] private GameObject bossMonsterPrefab;
    public float waitTimeAfterBoard = 3.0f;

    private GameBoardGenerator boardGenerator;
    private bool hasStartedWave = false;
    public int amountPerTime;

    public void Initialize(GameBoardGenerator generator)
    {
        boardGenerator = generator;
    }

    void Update()
    {
        var state = GameManager.Instance.CurrentState;
        if (state == GameFlowState.GAME_OVER || state == GameFlowState.GAME_CLEAR) return;
        if (GameManager.Instance.IsWaitingForClear) return;

        if (GameManager.Instance.CurrentState == GameFlowState.NORMAL_MONSTER_SPAWN)
        {
            amountPerTime = MonsterStats.NORMAL_MONSTER_Number;
        }
        else if (GameManager.Instance.CurrentState == GameFlowState.ELITE_MONSTER_SPAWN)
        {
            amountPerTime = MonsterStats.ELITE_MONSTER_Number;
        }
        else if (GameManager.Instance.CurrentState == GameFlowState.BOSS_MONSTER_SPAWN)
        {
            amountPerTime = MonsterStats.BOSS_MONSTER_Number;
        }

        if (!hasStartedWave && boardGenerator != null && boardGenerator.IsGameBoardGenerated)
        {
            if (boardGenerator.GameBoard != null && boardGenerator.GameBoard.transform.childCount > 0)
            {
                if (GameManager.Instance.CurrentState == GameFlowState.BEFORE_GATE_OPEN)
                {
                    hasStartedWave = false;
                    return;
                }

                if (GameManager.Instance.CurrentState == GameFlowState.GAME_START)
                {
                    return;
                }

                hasStartedWave = true;
                StartCoroutine(SimpleSpawnSequence());
            }
        }
    }

    IEnumerator SimpleSpawnSequence()
    {
        /*Vector3 boardCenterPos = boardGenerator.GameBoard.transform.position;

        GameObject player = GameObject.FindWithTag("Player");
        Vector3 spawnPos = boardCenterPos;
        Quaternion spawnRot = Quaternion.identity;

        if (player != null)
        {
            Vector3 dirFromPlayer = (boardCenterPos - player.transform.position).normalized;
            dirFromPlayer.y = 0;

            spawnPos = boardCenterPos + (dirFromPlayer * 0.5f);
            spawnRot = Quaternion.LookRotation(-dirFromPlayer);
        }

        // 게이트가 프리팹에 포함되어 있으므로 스포너 오브젝트 자체를 해당 위치로 이동
        transform.SetPositionAndRotation(spawnPos, spawnRot);*/

        // yield return new WaitForSeconds(waitTimeAfterBoard);

        GameObject monsterPrefab = GameManager.Instance.CurrentState switch
        {
            GameFlowState.NORMAL_MONSTER_SPAWN => normalMonsterPrefab,
            GameFlowState.ELITE_MONSTER_SPAWN  => eliteMonsterPrefab,
            GameFlowState.BOSS_MONSTER_SPAWN   => bossMonsterPrefab,
            _                                  => null
        };

        if (monsterPrefab == null)
        {
            Debug.LogWarning("[EnemySpawnerClone] 현재 상태에 맞는 몬스터 프리팹이 없습니다.");
            yield break;
        }

        for (int i = 0; i < amountPerTime; i++)
        {
            Vector3 monsterPos = transform.position + (transform.forward * 0.1f) + new Vector3(0, 0.1f, 0);
            
            if (GameManager.Instance.CurrentState == GameFlowState.NORMAL_MONSTER_SPAWN)
            {
                for (int j = 0; j <= GameManager.Instance.Phase; j++)
                {
                    Instantiate(monsterPrefab, monsterPos, transform.rotation);
                    Debug.Log(GameManager.Instance.Phase);
                    yield return new WaitForSeconds(0.2f);
                }
            }
            else
            {
                Instantiate(monsterPrefab, monsterPos, transform.rotation);
            }
            
            
            yield return new WaitForSeconds(1.0f);
        }

        if (GameManager.Instance.CurrentState == GameFlowState.BOSS_MONSTER_SPAWN)
        {
            GameManager.Instance.IsWaitingForClear = true;
            // 보스가 2초 대기 중에 이미 처치된 경우를 대비해 즉시 체크
            GameManager.Instance.TryAdvanceAfterBoss();
        }
        else
        {
            GameManager.Instance.Wave++;
        }
        hasStartedWave = false;
    }
}
