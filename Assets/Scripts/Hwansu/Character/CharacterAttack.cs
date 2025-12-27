using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackInterval = 1.5f;
    [SerializeField] private LayerMask monsterLayer;

    [Header("Attack Particle")]
    [SerializeField] private GameObject attackParticlePrefab;
    [SerializeField] private float particleSpawnInterval = 0.5f; // 이펙트 생성 주기
    [SerializeField] private float particleTravelTime = 0.5f; // 목표까지 도달 시간
    [SerializeField] private float particleLifetime = 2f;
    [SerializeField] private Vector2 particleSpawnOffset = new Vector2(0.5f, 0f);

    private float attackTimer = 0f;
    private float particleTimer = 0f; // 이펙트 생성 타이머
    private Monster currentTarget;

    private void Update()
    {
        FindTargetInRange();
        if (currentTarget != null)
        {
            float distance = Vector2.Distance(transform.position, currentTarget.transform.position);
            if (distance <= attackRange)
            {
                // 공격 타이머
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackInterval)
                {
                    Attack();
                    attackTimer = 0f;
                }

                // 이펙트 생성 타이머
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
            // 몬스터에 데미지
            currentTarget.TakeDamage();
        }
    }

    private void SpawnAttackParticle()
    {
        if (attackParticlePrefab == null) return;

        // 타겟까지의 거리 계산
        float distance = Vector2.Distance(transform.position, currentTarget.transform.position);

        // 거리와 시간으로 속도 계산
        float calculatedSpeed = distance / particleTravelTime;

        // 타겟 방향 계산
        Vector2 direction = (currentTarget.transform.position - transform.position).normalized;

        // 파티클 생성 위치
        Vector2 spawnPos = (Vector2)transform.position + particleSpawnOffset;

        // 파티클 생성
        GameObject particle = Instantiate(attackParticlePrefab, spawnPos, Quaternion.identity);

        // 파티클 이동 스크립트 추가
        AttackParticle particleScript = particle.AddComponent<AttackParticle>();
        particleScript.Initialize(direction, calculatedSpeed, particleLifetime, monsterLayer);
    }

    public void SetTarget(Monster target)
    {
        currentTarget = target;
        attackTimer = 0f;
        particleTimer = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        // 공격 범위 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}