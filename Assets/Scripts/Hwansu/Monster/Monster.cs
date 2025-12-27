using UnityEngine;
using System;
using System.Collections;

public class Monster : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;

    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float detectionRange = 10f;

    [Header("공격 설정")]
    [SerializeField] private float attackInterval = 4f; // 공격 간격
    [SerializeField] private float attackDistance = 1f; // 몸통박치기 이동 거리
    [SerializeField] private float attackSpeed = 10f; // 몸통박치기 속도

    [Header("사망 시 효과")]
    [SerializeField] private int goldReward = 50;

    [Header("깜빡임 효과")]
    [SerializeField] private float blinkInterval = 0.6f;
    [SerializeField] private float blinkDuration = 0.1f;

    public event Action OnMonsterDeath;

    private Transform playerTransform;
    private bool isWaiting = false;
    private Vector2 waitPosition;
    private MonsterSpawner spawner;
    private int initialMaxHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor = Color.white;

    private float attackTimer = 0f;
    private bool isAttacking = false;
    private bool canAttack = false; // minDistance 도달 여부

    public void Initialize(MonsterSpawner monsterSpawner)
    {
        spawner = monsterSpawner;
        initialMaxHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        StatBar[] statBars = UnityEngine.Object.FindObjectsByType<StatBar>(FindObjectsSortMode.None);

        int totalMaxCount = 0;

        foreach (StatBar statBar in statBars)
        {
            totalMaxCount += statBar.GetMaxCount();
            statBar.OnMaxReached += OnStatMaxReached;
        }

        maxHealth = Mathf.Max(1, maxHealth - totalMaxCount);
        currentHealth = maxHealth;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        StartCoroutine(BlinkRoutine());
    }

    private void OnDestroy()
    {
        StatBar[] statBars = UnityEngine.Object.FindObjectsByType<StatBar>(FindObjectsSortMode.None);

        foreach (StatBar statBar in statBars)
        {
            if (statBar != null)
            {
                statBar.OnMaxReached -= OnStatMaxReached;
            }
        }
    }

    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(blinkInterval);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.red;
            }

            yield return new WaitForSeconds(blinkDuration);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }

    private void OnStatMaxReached()
    {
        maxHealth = Mathf.Max(1, maxHealth - 1);

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    private void Update()
    {
        if (isAttacking)
            return;

        if (isWaiting)
        {
            MoveToWaitPosition();
        }
        else if (playerTransform != null)
        {
            ChasePlayer();

            // minDistance 안에 있으면 공격 타이머 작동
            if (canAttack)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackInterval)
                {
                    attackTimer = 0f;
                    StartCoroutine(PerformAttack());
                }
            }
        }
    }

    public void SetWaitMode(bool wait, Vector2 position)
    {
        isWaiting = wait;
        waitPosition = position;
    }

    private void MoveToWaitPosition()
    {
        float distance = Mathf.Abs(transform.position.x - waitPosition.x);
        if (distance > 0.1f)
        {
            float newX = Mathf.MoveTowards(transform.position.x, waitPosition.x, moveSpeed * Time.deltaTime);
            transform.position = new Vector2(newX, transform.position.y);

            float direction = Mathf.Sign(waitPosition.x - transform.position.x);
            float angle = direction > 0 ? 0 : 180;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void ChasePlayer()
    {
        float distance = Mathf.Abs(transform.position.x - playerTransform.position.x);

        if (distance <= minDistance)
        {
            // minDistance 안에 도달 - 공격 가능 상태
            canAttack = true;

            // 플레이어 방향으로 회전만 유지
            float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
            float angle = direction > 0 ? 0 : 180;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else if (distance <= detectionRange)
        {
            // 추적 범위 안 - 이동
            canAttack = false;
            attackTimer = 0f; // 타이머 리셋

            float newX = Mathf.MoveTowards(transform.position.x, playerTransform.position.x, moveSpeed * Time.deltaTime);
            transform.position = new Vector2(newX, transform.position.y);

            float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
            float angle = direction > 0 ? 0 : 180;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // 범위 밖
            canAttack = false;
            attackTimer = 0f;
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // 플레이어 방향 계산
        float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);

        // 시작 위치 저장
        Vector2 startPos = transform.position;
        Vector2 attackPos = startPos + new Vector2(direction * attackDistance, 0);

        // 앞으로 돌진
        float elapsed = 0f;
        float dashTime = attackDistance / attackSpeed;

        while (elapsed < dashTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dashTime;
            transform.position = Vector2.Lerp(startPos, attackPos, t);
            yield return null;
        }

        // 짧은 대기
        yield return new WaitForSeconds(0.1f);

        // 원래 위치로 복귀
        elapsed = 0f;
        while (elapsed < dashTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dashTime;
            transform.position = Vector2.Lerp(attackPos, startPos, t);
            yield return null;
        }

        transform.position = startPos;
        isAttacking = false;
    }

    public void TakeDamage()
    {
        currentHealth -= 1;
        currentHealth = Mathf.Max(0, currentHealth);
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (GoldManager.Instance != null)
            GoldManager.Instance.AddGold(goldReward);

        if (spawner != null)
            spawner.OnMonsterDeath(this);

        OnMonsterDeath?.Invoke();
        Destroy(gameObject);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}