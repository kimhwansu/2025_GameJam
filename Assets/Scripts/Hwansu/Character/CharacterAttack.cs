using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackInterval = 1.5f;
    [SerializeField] private LayerMask monsterLayer;

    [Header("Attack Particle")]
    [SerializeField] private GameObject attackParticlePrefab;
    [SerializeField] private float particleSpawnInterval = 0.5f;
    [SerializeField] private float particleTravelTime = 0.5f;
    [SerializeField] private float particleLifetime = 2f;
    [SerializeField] private Vector2 particleSpawnOffset = new Vector2(0.5f, 0f);

    private float attackTimer = 0f;
    private float particleTimer = 0f;
    private Monster currentTarget;

    private void Start()
    {
        // EffectManager에서 현재 이펙트 가져오기
        if (EffectManager.Instance != null)
        {
            GameObject currentEffect = EffectManager.Instance.GetCurrentEffectPrefab();
            if (currentEffect != null)
            {
                attackParticlePrefab = currentEffect;
            }
        }
    }

    private void Update()
    {
        FindTargetInRange();

        if (currentTarget != null)
        {
            float distance = Vector2.Distance(transform.position, currentTarget.transform.position);

            if (distance <= attackRange)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackInterval)
                {
                    Attack();
                    attackTimer = 0f;
                }

                particleTimer += Time.deltaTime;
                if (particleTimer >= particleSpawnInterval)
                {
                    SpawnAttackParticle();
                    particleTimer = 0f;
                }
            }
            else
            {
                currentTarget = null;
                attackTimer = 0f;
                particleTimer = 0f;
            }
        }
    }

    private void FindTargetInRange()
    {
        if (currentTarget != null) return;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange, monsterLayer);

        if (hitColliders.Length > 0)
        {
            float closestDistance = Mathf.Infinity;
            Monster closestMonster = null;

            foreach (Collider2D collider in hitColliders)
            {
                Monster monster = collider.GetComponent<Monster>();

                if (monster != null)
                {
                    float distance = Vector2.Distance(transform.position, collider.transform.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestMonster = monster;
                    }
                }
            }

            if (closestMonster != null)
            {
                SetTarget(closestMonster);
            }
        }
    }

    private void Attack()
    {
        if (currentTarget != null)
        {
            currentTarget.TakeDamage();
        }
    }

    private void SpawnAttackParticle()
    {
        if (attackParticlePrefab == null) return;

        float distance = Vector2.Distance(transform.position, currentTarget.transform.position);
        float calculatedSpeed = distance / particleTravelTime;
        Vector2 direction = (currentTarget.transform.position - transform.position).normalized;
        Vector2 spawnPos = (Vector2)transform.position + particleSpawnOffset;

        GameObject particle = Instantiate(attackParticlePrefab, spawnPos, Quaternion.identity);

        AttackParticle particleScript = particle.AddComponent<AttackParticle>();
        particleScript.Initialize(direction, calculatedSpeed, particleLifetime, monsterLayer);
    }

    // EffectManager에서 호출 - 이펙트 프리팹 업데이트
    public void UpdateEffectPrefab(GameObject newEffectPrefab)
    {
        attackParticlePrefab = newEffectPrefab;
        Debug.Log("공격 이펙트가 업데이트되었습니다!");
    }

    public void SetTarget(Monster target)
    {
        currentTarget = target;
        attackTimer = 0f;
        particleTimer = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}