using UnityEngine;
using static TurretStats;

public class Turret : MonoBehaviour
{
    protected TurretType type;

    [Header("Health Point Variables / Properties")]
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

    [Header("Combat Stat Variables / Properties")]
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

    [Header("Animator")]
    [SerializeField] protected Animator animator;

    [Header("Private Fields")]
    private Transform target;           // Current monster target to attack
    private float fireCooldown = 0f;    // Remaining cooldown time until the next attack
    private AudioSource audioSource;    // Audio source for attack sounds

    void Awake()
    {
        Init();
        audioSource = GetComponent<AudioSource>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
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

    /// <summary>
    /// Virtual function to initialize base turret stats (range, etc.)
    /// </summary>
    protected virtual void Init()
    {
        attackRange = TURRET_ATTACK_RANGE;
    }

    /// <summary>
    /// Increases turret HP and attack damage, and increments the upgrade count
    /// </summary>
    public void Upgrade()
    {
        maxHp += 10f;
        attackDamage += 10f;
        hp = maxHp;
        UpgradeCount++;

        Debug.Log($"{name} Upgraded: maxHp={maxHp}, attackDamage={attackDamage}, upgradeCount={UpgradeCount}");
    }

    /// <summary>
    /// Searches for the closest monster within attack range and sets it as the target
    /// </summary>
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

    /// <summary>
    /// Smoothly rotates the turret towards the target monster
    /// </summary>
    private void RotateToMonster()
    {
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        dir.Normalize();
        Quaternion lookRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
    }

    /// <summary>
    /// Deals damage to the target monster and plays attack animations/sounds
    /// </summary>
    private void Attack()
    {
        if (target == null) return;

        if (audioSource != null)
            audioSource.Play();

        PlayAttackAnimation();

        Monster monster = target.GetComponent<Monster>();
        monster.TakeDamage(attackDamage);

        Debug.Log($"{name} attack - damage : {attackDamage}, remaining HP: {monster.HP}");
    }

    /// <summary>
    /// Virtual function to execute the turret's attack animation
    /// </summary>
    protected virtual void PlayAttackAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Shoot");
    }

    /// <summary>
    /// Called when the turret takes damage; destroys the object if HP falls to 0 or below
    /// </summary>
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