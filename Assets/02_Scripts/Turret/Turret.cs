using UnityEngine;
using static TurretStats;

public class Turret : MonoBehaviour
{
    protected TurretType type;

    [Header("Health Point 변수 / 프로퍼티")]
    protected float hp;
    public float HP
    {
        get
        {
            return hp;
        }

        set
        {
            hp = value;
        }
    }

    [Header("포탑의 공격 수치에 관련한 변수 / 프로퍼티")]
    protected float attackDamage;
    protected float attackSpeed;
    protected float attackRange;

    public float AttackDamage
    {
        get
        {
            return attackDamage;
        }

        set
        {
            attackDamage = value;
        }
    }

    public float AttackSpeed
    {
        get
        {
            return attackSpeed;
        }

        set
        {
            attackSpeed = value;
        }
    }

    public float AttackRange
    {
        get
        {
            return attackRange;
        }

        set
        {
            attackRange = value;
        }
    }

    [Header("Private")]
    private Transform target;
    private float fireCooldown = 0f;

    void Awake()
    {
        Init();
    }

    void Update()
    {
        FindMonster();
        if (target == null) return;

        RotateToMonster();

        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Attack();
            fireCooldown = 1f / attackSpeed;
        }
    }

    protected virtual void Init()
    {
        hp = TURRET_HP;
        attackDamage = TURRET_ATTACK_DAMAGE;
        attackRange = TURRET_ATTACK_RANGE;
        attackSpeed = TURRET_ATTACK_SPEED;

        Debug.Log($"{name}: hp - {hp}, damage - {attackDamage}, range - {attackRange}, speed - {attackSpeed}");
    }

    private void FindMonster()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider col in hits)
        {
            if (!col.CompareTag("Enemy")) continue;

            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = col.transform;
            }
        }

        target = closest;
    }

    private void RotateToMonster()
    {
        if (target == null) return;

        Vector3 dir = (target.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
    }

    private void Attack()
    {
        if (target == null) return;

        Monster monster = target.GetComponent<Monster>();
        monster.TakeDamage(attackDamage);

        Debug.Log($"{name} - damage : {attackDamage}, remaining HP: {monster.HP}");
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"{name} : attacked - remaining HP : {hp}");

        hp -= damage;
        if (hp <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
