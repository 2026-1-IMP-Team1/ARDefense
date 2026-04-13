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

    void Update()
    {
        if (!hasStartedWave && boardGenerator != null && boardGenerator.IsGameBoardGenerated)
        {
            if (boardGenerator.GameBoard != null && boardGenerator.GameBoard.transform.childCount > 0)
            {
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
    }
}