using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    [Header("몬스터가 일반/엘리트/보스 몬스터 중 어떤 몬스터인지? 본인의 타입을 명시하는 변수 / 프로퍼티")]

    protected MonsterType type;

    public MonsterType Type
    {
        get
        {
            return type;
        }

        // 일단 Monster의 Type은 외부에서 함부로 변경하기 어렵도록 하는 게 좋을 거 같아 private set으로 해놓았습니다!
        private set { }
    }

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

    [Header("몬스터의 공격 수치에 관련한 변수 / 프로퍼티")]

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

    public abstract void Init();

    // 실시간 몬스터 체력 관리 
    private void Update()
    {
        if (hp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // 1. 타입에 따른 골드 양 결정
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
                // 보스 몬스터가 죽으면 세대가 넘어가도록 했습니다.
                GameManager.Instance.phase++; 
                break;
        }

        // 2. GoldManager의 싱글톤 인스턴스를 통해 골드 추가
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.AddGold(rewardGold);
        }

        // 3. 몬스터 오브젝트 파괴
        Destroy(gameObject);
    }

}
