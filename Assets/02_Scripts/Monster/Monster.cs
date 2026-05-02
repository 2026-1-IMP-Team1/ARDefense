using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    [Header("Variable / Property that specifies the monster's type (Normal/Elite/Boss).")]

    protected MonsterType type;

    public MonsterType Type
    {
        get
        {
            return type;
        }

        // For now, I've made the Monster's Type have a private set to make it difficult to change from the outside!
        private set { }
    }

    [Header("Health Point variable / property")]

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

    protected bool isDead; // Monster's survival status (true: dead, false: alive)

    [Header("Variables / Properties related to the monster's attack stats")]

    protected float attackDamage;
    protected int attackSpeed;

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

    public int AttackSpeed
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

    // A virtual function that initializes stats for the monster type. It is overridden and used in child classes.
    public virtual void Init() {}

    protected bool isCountedAsAlive = false; // Whether it is included in the count of living monsters in GameManager (prevents duplicate counting)

    protected virtual void Awake()
    {
        // When a monster is created, its stats are initialized and it is registered as a living monster in the GameManager.
        Init();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMonster();
            isCountedAsAlive = true;
        }
        Debug.Log($"{name} : hp - {hp}, attackDamage - {attackDamage}, attackSpeed - {attackSpeed}");
    }



    // This is the function for when a monster takes damage.
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        hp -= damage;
        if (hp <= 0)
        {
            isDead = true;
            Die();
            Debug.Log($"{name} die");
        }
    }

    // Called when the monster dies. It gives a gold reward, reduces the monster count, and then destroys the object.
    protected virtual void Die()
    {
        // 1. Determine the amount of gold based on the type
        int rewardGold = 0;
        switch (type)
        {
            case MonsterType.NORMAL_MONSTER:
                rewardGold = Gold.NORMAL_MONSTER_GOLD;
                break;
            case MonsterType.ELITE_MONSTER:
                rewardGold = Gold.ELITE_MONSTER_GOLD;
                break;
            case MonsterType.BOSS_MONSTER:
                rewardGold = Gold.BOSS_MONSTER_GOLD;
                break;
        }

        // 2. Add gold through the singleton instance of GoldManager
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.AddGold(rewardGold);
        }

        // Reflect the number of living monsters in GameManager
        if (isCountedAsAlive && GameManager.Instance != null)
        {
            GameManager.Instance.RemoveMonster();
            isCountedAsAlive = false;
        }

        // 3. Destroy the monster object
        Destroy(gameObject);
    }

    // To prevent count leaks, remove from the living monster count in case the object is destroyed abnormally.
    private void OnDestroy()
    {
        if (isCountedAsAlive && GameManager.Instance != null)
        {
            GameManager.Instance.RemoveMonster();
            isCountedAsAlive = false;
        }
    }
}
