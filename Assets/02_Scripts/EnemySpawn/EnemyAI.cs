using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 1.0f;          // Monster's movement speed
    public float attackRange = 0.2f;    // Maximum attack range radius for the target
    private Transform target;           // The current target the monster is tracking (turret or player)
    private Monster monster;            // Component to get the monster's stats (attack power, attack speed, etc.)
    private float attackCooldown = 2f;  // Cooldown variable for waiting until the next attack
    private AudioSource audioSource;    // Audio source for sound effects to play on attack

    // Initializes necessary components when the object is created.
    void Awake()
    {
        monster = GetComponent<Monster>();
        audioSource = GetComponent<AudioSource>();
    }

    // Every frame, it searches for a target and executes movement and attack logic based on conditions.
    void Update()
    {
        if (GameManager.Instance != null)
        {
            var s = GameManager.Instance.CurrentState;
            if (s == GameFlowState.GAME_OVER || s == GameFlowState.GAME_CLEAR) return;
        }

        FindTarget(); // Find target (player or turret)

        if (target == null) // If there is no target, move straight forward
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        MoveToTarget();

        attackCooldown -= Time.deltaTime;
        if (Vector3.Distance(transform.position, target.position) <= attackRange && attackCooldown <= 0f) // Attack if within range
        {
            Attack();
            int spd = (monster != null && monster.AttackSpeed > 0) ? monster.AttackSpeed : 1;
            attackCooldown = 1f / spd;
        }
    }

    // Finds a target on the map. The first priority is to find the nearest turret, and if there are no turrets, the second priority is to target the player.
    void FindTarget()
    {
        // Priority 1: Find the nearest turret
        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
        float closestDist = Mathf.Infinity;
        GameObject nearestTurret = null;

        // Select the closest turret among all turrets
        foreach (GameObject t in turrets)
        {
            float dist = Vector3.Distance(transform.position, t.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                nearestTurret = t;
            }
        }

        // If a nearby turret exists, set it as the target
        if (nearestTurret != null)
        {
            target = nearestTurret.transform;
        }
        else
        {
            // Priority 2: If there are no turrets, find the player
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    // Smoothly rotates (Slerp) towards the target and moves towards it.
    void MoveToTarget()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0; // The y-axis is fixed for now!
        
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 0.1f);
        }
        // Move towards the target
        transform.position += dir * speed * Time.deltaTime;
    }

    // Checks the component of the target (player or turret) to inflict damage and play the attack sound.
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