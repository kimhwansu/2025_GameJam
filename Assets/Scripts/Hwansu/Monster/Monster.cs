using UnityEngine;
using System;

public class Monster : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;

    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float detectionRange = 10f;

    [Header("사망 시 효과")]
    [SerializeField] private int goldReward = 50;

    public event Action OnMonsterDeath;

    private Transform playerTransform;
    private bool isWaiting = false;
    private Vector2 waitPosition;
    private MonsterSpawner spawner;
    private int initialMaxHealth; // 초기 최대 체력 저장

    public void Initialize(MonsterSpawner monsterSpawner)
    {
        spawner = monsterSpawner;
        initialMaxHealth = maxHealth;

        // 모든 StatBar의 MAX 카운트 합산
        StatBar[] statBars = UnityEngine.Object.FindObjectsByType<StatBar>(FindObjectsSortMode.None);

        int totalMaxCount = 0;

        foreach (StatBar statBar in statBars)
        {
            totalMaxCount += statBar.GetMaxCount();
            statBar.OnMaxReached += OnStatMaxReached;
        }

        // 총 MAX 카운트만큼 최대 체력 감소
        maxHealth = Mathf.Max(1, maxHealth - totalMaxCount);
        currentHealth = maxHealth;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    private void OnDestroy()
    {
        // 모든 StatBar 이벤트 구독 해제
        StatBar[] statBars = UnityEngine.Object.FindObjectsByType<StatBar>(FindObjectsSortMode.None);

        foreach (StatBar statBar in statBars)
        {
            if (statBar != null)
            {
                statBar.OnMaxReached -= OnStatMaxReached;
            }
        }
    }

    private void OnStatMaxReached()
    {
        // 최대 체력 감소 (최소 1 유지)
        maxHealth = Mathf.Max(1, maxHealth - 1);

        // 현재 체력도 최대 체력을 초과하지 않도록 조정
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    private void Update()
    {
        if (isWaiting)
        {
            MoveToWaitPosition();
        }
        else if (playerTransform != null)
        {
            ChasePlayer();
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
            // X축으로만 이동, Y축은 고정
            float newX = Mathf.MoveTowards(transform.position.x, waitPosition.x, moveSpeed * Time.deltaTime);
            transform.position = new Vector2(newX, transform.position.y);

            // 이동 방향에 따라 회전
            float direction = Mathf.Sign(waitPosition.x - transform.position.x);
            float angle = direction > 0 ? 0 : 180;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void ChasePlayer()
    {
        // X축 거리만 계산
        float distance = Mathf.Abs(transform.position.x - playerTransform.position.x);

        if (distance <= detectionRange && distance > minDistance)
        {
            // X축으로만 이동, Y축은 고정
            float newX = Mathf.MoveTowards(transform.position.x, playerTransform.position.x, moveSpeed * Time.deltaTime);
            transform.position = new Vector2(newX, transform.position.y);

            // 이동 방향에 따라 회전
            float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
            float angle = direction > 0 ? 0 : 180;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
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

    // 현재 체력 정보 확인용
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}