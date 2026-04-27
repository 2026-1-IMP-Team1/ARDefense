using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 1.0f;
    public float attackRange = 0.2f; //사거리
    private Transform target;
    private Monster monster;
    private float attackCooldown = 2f;
    private AudioSource audioSource;

    void Awake()
    {
        monster = GetComponent<Monster>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameManager.Instance != null)
        {
            var s = GameManager.Instance.CurrentState;
            if (s == GameFlowState.GAME_OVER || s == GameFlowState.GAME_CLEAR) return;
        }

        FindTarget(); //타겟 찾기 (플레이어나 터렛)

        if (target == null) //타겟 없으면 앞으로 직진
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        MoveToTarget();

        attackCooldown -= Time.deltaTime;
        if (Vector3.Distance(transform.position, target.position) <= attackRange && attackCooldown <= 0f) //사거리 안쪽이면 공격
        {
            Attack();
            int spd = (monster != null && monster.AttackSpeed > 0) ? monster.AttackSpeed : 1;
            attackCooldown = 1f / spd;
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
        if (target == null) return;
        float damage = monster != null ? monster.AttackDamage : 1f;

        if (audioSource != null)
        {
            audioSource.Play();
        }

        Player player = target.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            return;
        }

        Turret turret = target.GetComponent<Turret>();
        if (turret != null)
        {
            turret.TakeDamage(damage);
        }
    }
}