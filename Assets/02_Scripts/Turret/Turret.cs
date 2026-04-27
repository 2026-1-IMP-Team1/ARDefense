using UnityEngine;
using static TurretStats;

public class Turret : MonoBehaviour
{
    protected TurretType type;

    [Header("Health Point 변수 / 프로퍼티")]
    protected float hp;
    protected float maxHp;

    public float HP
    {
        get { return hp; }
        set { hp = value; }
    }

    public float MaxHP
    {
        get { return maxHp; }
    }

    public int UpgradeCount { get; private set; } = 0;

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
    private AudioSource audioSource;

    void Awake()
    {
        Init();
        audioSource = GetComponent<AudioSource>();
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
        attackRange = TURRET_ATTACK_RANGE;
    }

    public void Upgrade()
    {
        maxHp += 10f;
        attackDamage += 10f;
        hp = maxHp;
        UpgradeCount++;

        Debug.Log($"{name} 업그레이드: maxHp={maxHp}, attackDamage={attackDamage}, upgradeCount={UpgradeCount}");
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

        if (audioSource != null)
        {
            audioSource.Play();
        }

        Monster monster = target.GetComponent<Monster>();
        monster.TakeDamage(attackDamage);

        Debug.Log($"{name} attack - damage : {attackDamage}, remaining HP: {monster.HP}");
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
