using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float maxHp = 100f;
    private float hp;

    public float HP => hp;
    public float MaxHP => maxHp;

    private void Awake()
    {
        hp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        if (GameManager.Instance.CurrentState == GameFlowState.GAME_OVER) return;

        hp = Mathf.Max(0f, hp - damage);
        Debug.Log($"[Player] HP: {hp}/{maxHp}");

        if (hp <= 0f) Die();
    }

    private void Die()
    {
        Debug.Log("[Player] Die - Game Over");
        GameManager.Instance.CurrentState = GameFlowState.GAME_OVER;
        Time.timeScale = 0;

        Destroy(gameObject);
    }
}
