using UnityEngine;
using System.Collections;

public class EnemySpawnerClone : MonoBehaviour
{
    [Header("Monster Prefabs to Spawn")]
    [SerializeField] private GameObject normalMonsterPrefab; // Normal monster prefab
    [SerializeField] private GameObject eliteMonsterPrefab;  // Elite monster prefab
    [SerializeField] private GameObject bossMonsterPrefab;   // Boss monster prefab

    [Header("Spawn Settings")]
    public float waitTimeAfterBoard = 3.0f; // Wait time after board generation until spawn starts
    public int amountPerTime;               // Number of monsters to spawn in the current wave

    private GameBoardGenerator boardGenerator; // Game board generator reference
    private bool hasStartedWave = false;       // Whether the monster spawn for the current wave has started

    // Initializes by receiving a reference to GameBoardGenerator from an external source (SceneController).
    public void Initialize(GameBoardGenerator generator)
    {
        boardGenerator = generator;
    }

    private void Update()
    {
        // Checks the game state every frame to decide whether to start the monster spawn coroutine.
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

    // A coroutine that spawns a set number of monsters appropriate for the current game state.
    private IEnumerator SimpleSpawnSequence()
    {
        // Selects the monster prefab to spawn based on the current game state.
        GameObject monsterPrefab = GameManager.Instance.CurrentState switch
        {
            GameFlowState.NORMAL_MONSTER_SPAWN => normalMonsterPrefab,
            GameFlowState.ELITE_MONSTER_SPAWN  => eliteMonsterPrefab,
            GameFlowState.BOSS_MONSTER_SPAWN   => bossMonsterPrefab,
            _                                  => null
        };

        if (monsterPrefab == null)
        {
            Debug.LogWarning("[EnemySpawnerClone] No monster prefab available for the current state.");
            yield break;
        }

        // Spawns monsters up to the specified number (amountPerTime).
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

        // After spawning is finished, it either proceeds to the next wave or switches to a waiting-for-clear state, depending on the game state.
        if (GameManager.Instance.CurrentState == GameFlowState.BOSS_MONSTER_SPAWN)
        {
            GameManager.Instance.IsWaitingForClear = true;
            // Immediately check in case the boss is already defeated during the 2-second wait
            GameManager.Instance.TryAdvanceAfterBoss();
        }
        else
        {
            GameManager.Instance.Wave++;
        }
        hasStartedWave = false;
    }
}
