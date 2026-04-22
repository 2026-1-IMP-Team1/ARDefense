using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 1.0f;
    public float attackRange = 0.2f; //사거리
    private Transform target;

    void Update()
    {
        FindTarget(); //타겟 찾기 (플레이어나 터렛)

        if (target == null) //타겟 없으면 앞으로 직진
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        MoveToTarget();

        if (Vector3.Distance(transform.position, target.position) <= attackRange) //사거리 안쪽이면 공격
        {
            Attack();
        }
    }

    void FindTarget()
    {
        // 1순위: 가장 가까운 터렛 찾기
        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
        float closestDist = Mathf.Infinity;
        GameObject nearestTurret = null;

        // 모든 터렛 중 가장 가까운 터렛을 선별하기
        foreach (GameObject t in turrets)
        {
            float dist = Vector3.Distance(transform.position, t.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                nearestTurret = t;
            }
        }

        // 가까운 터렛이 있다면 타겟으로 설정
        if (nearestTurret != null)
        {
            target = nearestTurret.transform;
        }
        else
        {
            // 2순위: 터렛이 없으면 플레이어 찾기
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    // 타겟을 향해 몸을 돌리고 이동
    void MoveToTarget()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0; //y축은 일단 고정시켰습니다!
        
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 0.1f);
        }
        //타겟 방향으로 이동
        transform.position += dir * speed * Time.deltaTime;
    }

    void Attack()
    {
        // 여기에 공격 애니메이션이나 데미지 로직 추가하면 됩니다
        Debug.Log($"{target.name} 공격 중!");
    }
}