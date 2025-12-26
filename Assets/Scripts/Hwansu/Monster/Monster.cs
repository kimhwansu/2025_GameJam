using UnityEngine;
using System;

public class Monster : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;   //최대 체력
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

    public void Initialize(MonsterSpawner monsterSpawner)
    {
        spawner = monsterSpawner;
        currentHealth = maxHealth;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
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
        float distance = Vector2.Distance(transform.position, waitPosition);

        // 대기 위치에 거의 도착하면 멈춤
        if (distance > 0.1f)
        {
            Vector2 direction = (waitPosition - (Vector2)transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, waitPosition, moveSpeed * Time.deltaTime);

            // 회전
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void ChasePlayer()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= detectionRange && distance > minDistance)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
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
}