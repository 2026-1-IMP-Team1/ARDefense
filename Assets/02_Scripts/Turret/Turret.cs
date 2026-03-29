using UnityEngine;

public abstract class Turret : MonoBehaviour
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
