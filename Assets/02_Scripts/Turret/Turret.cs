using UnityEngine;

public abstract class Turret : MonoBehaviour
{
    protected TurretType type;

    [Header("Health Point 변수 / 프로퍼티")]
    protected float hp;

    // 포탑 가격(타입에 따라서, 세대에 따라서 달라지게 만들어야 함)
    // 일단 임시로 30으로 정해놓았습니다.
    public int turretCost = 30;

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
}
